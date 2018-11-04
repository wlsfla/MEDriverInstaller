using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.Text.RegularExpressions;

namespace ME_Driver_Installer
{
	class Program
	{
		static void Main(string[] args)
		{
			//ManagementScope scope = new ManagementScope(@"\\.\ROOT\cimv2");
			//ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_BaseBoard");// Where Tag=\"Base Board\"
			//ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
			//ManagementObjectCollection queryCollection = searcher.Get();
			//foreach (ManagementObject m in queryCollection) { }

			DotNetInstaller ins = new DotNetInstaller("");
			ins.Run();
			if (ins.IsDotNetInstalled)
			{
				MEDriverInstaller me = new ME_Driver_Installer.MEDriverInstaller();
				me.Run();
			}

			Console.WriteLine("\n\nEnd");
			Console.Read();
		}
	}

	/// <summary>
	/// 닷넷 4.5 설치여부 검사 및 다운로드/설치
	/// </summary>
	class DotNetInstaller
	{
		public DotNetInstaller(string url)
		{
			this.URL = url;
		}

		public void Run()
		{
			if (this.IsDotNetInstalled)
			{
				this.GetDownloadFile(this.URL);
				this.ExecFiles(Path);
			}
		}

		private void ExecFiles(string path)
		{

		}

		private void GetDownloadFile(string url)
		{

		}

		/// <summary>
		/// 닷넷 4.5 미만일 때 닷넷 4.5 설치
		/// </summary>
		public bool IsDotNetInstalled { get { return true; } }

		private string URL { get; set; }

		private string Path { get; set; }
	}

	public class MEDriverInstaller
	{
		public void Run()
		{
			if (!this.IsMEinstalled)
			{

			}
		}

		private void Install()
		{

		}

		/// <summary>
		/// 완료
		/// 메인보드 Product Name
		/// </summary>
		public string MainboardProductName {
			get
			{
				string result = string.Empty;
				ManagementScope scope = new ManagementScope(@"\\.\ROOT\cimv2");
				ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_BaseBoard");
				ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
				ManagementObjectCollection queryCollection = searcher.Get();

				foreach (ManagementObject m in queryCollection)
				{
					// access properties of the WMI object
					result = m["Product"] as string;
				}
				return result;
			}
		}

		public bool IsMEinstalled { get; }

		/// <summary>
		/// 완료
		/// 인텔 chipset 목록 리턴
		/// </summary>
		private Dictionary<string, string[]> IntelChipsetList {
			get
			{
				Dictionary<string, string[]> dic = new Dictionary<string, string[]>();
				// Intel Consumer Chipsets
				dic.Add("300 Series", new string[] { "B360", "H370", "H310", "Q370", "Z390", "Z370", "HM370", "QM370" });
				dic.Add("200 Series", new string[] { "X299", "Z270", "Q270", "H270", "Q250", "B250" });
				dic.Add("100 Series", new string[] { "Z170", "Q170", "H170", "Q150", "B150", "H110", "HM175", "QM175", "HM170", "QM170" });
				dic.Add("9 Series", new string[] { "X99", "Z97", "H97" });
				dic.Add("8 Series", new string[] { "Z87", "Q87", "H87", "Q85", "B85", "H81", "HM87", "QM87", "HM86" });
				dic.Add("7 Series", new string[] { "X79", "Z77", "Q77", "H77", "Z75", "Q75", "B75", "HM77", "QM77", "UM77", "QS77", "HM76", "HM76", "HM70" });

				// Intel Server Chipsets(without Intel Communications Chipsets)
				dic.Add("C620 Series", new string[] { "C629", "C628", "C627", "C626", "C625", "C624", "C622", "C621" });
				dic.Add("C610 Series", new string[] { "C612" });
				dic.Add("C600 Series", new string[] { "C608", "C606", "C604", "C602J", "C602" });
				dic.Add("C400 Series", new string[] { "C400" });
				dic.Add("C240 Series", new string[] { "C246", "CM246" });
				dic.Add("C230 Series", new string[] { "CM238", "CM236", "C236", "C232" });
				dic.Add("C220 Series", new string[] { "C226", "C224", "C222" });
				dic.Add("C210 Series", new string[] { "C216" });
				dic.Add("C200 Series", new string[] { "C206", "C204", "C202" });

				return dic;
			}
		}

		/// <summary>
		/// 완료
		/// 메인보드 Chipset Name
		/// </summary>
		public string MainboardChipset {
			get
			{
				string result = string.Empty;
				string name = string.Empty;
				ManagementScope scope = new ManagementScope(@"\\.\ROOT\cimv2");
				ObjectQuery query = new ObjectQuery("SELECT Name FROM Win32_PnPEntity WHERE Name LIKE \"Intel(R) % Series Chipset Family % \"");
				ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
				ManagementObjectCollection queryCollection = searcher.Get();

				foreach (ManagementObject m in queryCollection)
					name = m["Name"] as string;
				result = Regex(@"Intel(R) (?P<series1>.+ Series)/(?P<series2>.+ Series) Chipset Family", name);
				return result;
			}
		}

		/// <summary>
		/// 완료
		/// </summary>
		private string Regex(string pattern, string strings)
		{
			Regex regex = new Regex(pattern);
			Match m = regex.Match(strings);
			if (m.Success)
				return m.Value;
			else
				return "Unkwon";
		}
	}
}
