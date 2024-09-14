namespace ThGameMgr.Lite
{
    /// <summary>
    /// GameTotalRunningTimeDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GameTotalRunningTimeDialog : Window
    {
        private string? _gameTotalRunningTime;

        public string? GameTotalRunningTime
        {
            get
            {
                return _gameTotalRunningTime;
            }

            set
            {
                _gameTotalRunningTime = value;
                if (!string.IsNullOrEmpty(value))
                {
                    GameTotalRunningTimeBlock.Text = value;
                }
            }
        }

        public GameTotalRunningTimeDialog()
        {
            InitializeComponent();
        }

        private void OKButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
