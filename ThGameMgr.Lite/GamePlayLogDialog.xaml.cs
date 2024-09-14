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

        private void GameTotalRunningTimeMenuItemClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<GamePlayLogData> gamePlayLogDatas
                = GameLogDataGrid.DataContext as ObservableCollection<GamePlayLogData>;

            int totalGameRunningTime = 0;

            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDatas)
            {
                try
                {
                    string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                    int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                    int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);
                    totalGameRunningTime += gameRunningTimeMin + gameRunningTimeSec;
                }
                catch (Exception)
                {

                }

                int totalMinutes = totalGameRunningTime / 60;
                int totalSeconds = totalGameRunningTime % 60;

                MessageBox.Show(this, $"{totalMinutes:00}min {totalSeconds:00}sec", "ゲーム実行時間の合計",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
