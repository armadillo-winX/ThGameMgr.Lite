namespace ThGameMgr.Lite
{
    internal class PathInfo
    {
        public static string AppPath => typeof(App).Assembly.Location;

        public static string? AppLocation => Path.GetDirectoryName(AppPath);

        public static string? SettingsDirectory => $"{AppLocation}\\Settings";

        public static string? GamePathSettingsFile => $"{SettingsDirectory}\\GamePathConfig.xml";

        public static string? MainWindowSettingsFile => $"{SettingsDirectory}\\MainWindowConfig.xml";

        public static string? GameSpecificConfigFile => $"{SettingsDirectory}\\GameSpecificConfig.xml";

        public static string? ResizerFrameSettingsFile => $"{SettingsDirectory}\\ResizerFrameWindowConfig.xml";

        public static string? GamePlayLogRecordFile => $"{AppLocation}\\GamePlayLog.xml";

        public static string? ReadMeFile => $"{AppLocation}\\ReadMe.txt";
    }
}
