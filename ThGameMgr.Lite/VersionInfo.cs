using System.Runtime.InteropServices;

namespace ThGameMgr.Lite
{
    internal class VersionInfo
    {
        public static readonly string _appPath = PathInfo.AppPath;

        public static string? AppName => FileVersionInfo.GetVersionInfo(_appPath).ProductName;

        public static string? AppVersion => FileVersionInfo.GetVersionInfo(_appPath).ProductVersion;

        public static string? Developer => FileVersionInfo.GetVersionInfo(_appPath).CompanyName;

        public static string? Copyright => FileVersionInfo.GetVersionInfo(_appPath).LegalCopyright;

        public static string OperatingSystem => RuntimeInformation.OSDescription;

        public static string DotNetRuntime => RuntimeInformation.FrameworkDescription;

        public static string SystemArchitecture => RuntimeInformation.OSArchitecture.ToString();

        public static string AppArchitecture => RuntimeInformation.ProcessArchitecture.ToString();
    }
}
