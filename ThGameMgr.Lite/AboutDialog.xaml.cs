namespace ThGameMgr.Lite
{
    /// <summary>
    /// AboutDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutDialog : Window
    {
        public AboutDialog()
        {
            InitializeComponent();

            AppNameBlock.Text = VersionInfo.AppName;
            AppVersionBlock.Text = $"Version.{VersionInfo.AppVersion}";
            DeveloperBlock.Text = $"by {VersionInfo.Developer}";
            SystemInfoBlock.Text = $"System: {VersionInfo.OperatingSystem}";
            DotNetVersionBlock.Text = VersionInfo.DotNetRuntime;
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
