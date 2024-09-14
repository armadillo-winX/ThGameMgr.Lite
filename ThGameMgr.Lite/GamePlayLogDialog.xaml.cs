using System.Collections.ObjectModel;

namespace ThGameMgr.Lite
{
    /// <summary>
    /// GamePlayLogDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GamePlayLogDialog : Window
    {
        public GamePlayLogDialog()
        {
            InitializeComponent();


            try
            {
                ViewGamePlayLogData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲーム実行履歴の取得に失敗しました。\n{ex.Message}",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewGamePlayLogData()
        {
            string gamePlayLogRecordFile = PathInfo.GamePlayLogRecordFile;
            if (File.Exists(gamePlayLogRecordFile))
            {
                ObservableCollection<GamePlayLogData> gamePlayLogDatas = [];
                gamePlayLogDatas = GamePlayLogRecorder.GetGamePlayLogDatas();
                GameLogDataGrid.AutoGenerateColumns = false;
                GameLogDataGrid.DataContext = gamePlayLogDatas;
            }
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
