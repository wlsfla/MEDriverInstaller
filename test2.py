import wmi
import re

intelChipset = {
    # Intel Consumer Chipsets
    '300 Series' : ['B360','H370','H310','Q370','Z390','Z370','HM370','QM370'],
    '200 Series' : ['X299','Z270','Q270','H270','Q250','B250'],
    '100 Series' : ['Z170','Q170','H170','Q150','B150','H110','HM175','QM175','HM170','QM170'],
    '9 Series' : ['X99','Z97','H97'],
    '8 Series' : ['Z87','Q87','H87','Q85','B85','H81','HM87','QM87','HM86'],
    '7 Series' : ['X79','Z77','Q77','H77','Z75','Q75','B75','HM77','QM77','UM77','QS77','HM76','HM76','HM70'],
    # Intel Server Chipsets(without Intel Communications Chipsets)
    'C620 Series' : ['C629','C628','C627','C626','C625','C624','C622','C621'],
    'C610 Series' : ['C612'],
    'C600 Series' : ['C608','C606','C604','C602J','C602'],
    'C400 Series' : ['C400'],
    'C240 Series' : ['C246','CM246'],
    'C230 Series' : ['CM238','CM236','C236','C232'],
    'C220 Series' : ['C226','C224','C222'],
    'C210 Series' : ['C216'],
    'C200 Series' : ['C206','C204','C202']
}

class checkSystem():
    def __init__(self):
        self.w = wmi.WMI(find_classes=False)
        self.mainboardProduct = False
        self.mainboardManufature = False
        self.mainboardChipset = False
        self.flagMEDriverInstall = False
        self.flagMEDriverVersion = False
        self.regChipset = re.compile('Intel\\(R\\) (?P<series1>.+ Series)/(?P<series2>.+ Series) Chipset Family')
        self.checkMainBoard()
        self.checkMEDriverVersion()
        self.checkChipset()

    def checkMainBoard(self):
        baseboard = self.w.Win32_BaseBoard(['Manufacturer','Product'])[0]
        self.mainboardProduct = baseboard.wmi_property('Product').value
        self.mainboardManufature = baseboard.wmi_property('Manufacturer').value

    def checkMEDriverVersion(self):
        result = self.w.query('SELECT DriverVersion FROM Win32_PnPSignedDriver WHERE DeviceName LIKE "Intel(R) Management Engine Interface%"')
        if result:
            self.flagMEDriverInstall = True
            self.flagMEDriverVersion = result[0].wmi_property('DriverVersion').value

    def checkChipset(self):
        # Check by mainboard product name.
        for chipsetSeires in intelChipset.keys():
            for series in intelChipset[chipsetSeires]:
                if series in self.mainboardProduct:
                    self.mainboardChipset = chipsetSeires
                    return
        # Check by device name.
        result = self.w.query('SELECT Name FROM Win32_PnPEntity WHERE Name LIKE "Intel(R) % Series Chipset Family%"')
        if result:
            for r in result:
                reg = self.regChipset.search(r.wmi_property('Name').value)
                if reg:
                    self.mainboardChipset = '{}/{}'.format(reg.group('series1'), reg.group('series2'))
                    break

    def printResult(self):
        if self.mainboardProduct and self.mainboardManufature:
            print('[+] Mainboard : {}({})'.format(self.mainboardProduct, self.mainboardManufature))
        else:
            print('[-] Mainboard : IDK!!')

        if self.mainboardChipset:
            print('[+] Chipset : {}'.format(self.mainboardChipset))
        else:
            print('[-] Chipset : IDK!!')

        if self.flagMEDriverInstall and self.flagMEDriverVersion:
            print('[+] Intel ME Driver : Already Installed(v{})'.format(self.flagMEDriverVersion))
        else:
            print('[+] Intel ME Driver : Need to Install')


if __name__ == '__main__':
    obj = checkSystem()
    obj.printResult()