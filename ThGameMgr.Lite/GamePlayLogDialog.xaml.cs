﻿using System.Collections.ObjectModel;

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
                ObservableCollection<GamePlayLogData> gamePlayLogDataCollection = [];
                gamePlayLogDataCollection = GamePlayLogRecorder.GetGamePlayLogDataCollection();
                GameLogDataGrid.AutoGenerateColumns = false;
                GameLogDataGrid.DataContext = gamePlayLogDataCollection;
            }
        }

        private void CloseMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GameTotalRunningTimeMenuItemClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<GamePlayLogData> gamePlayLogDataCollection
                = GameLogDataGrid.DataContext as ObservableCollection<GamePlayLogData>;

            int totalGameRunningTime = 0;

            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDataCollection)
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
            }


            int totalMinutes = totalGameRunningTime / 60;
            int totalSeconds = totalGameRunningTime % 60;

            GameTotalRunningTimeDialog gameTotalRunningTimeDialog = new()
            {
                Owner = this,
                GameTotalRunningTime = $"{totalMinutes:00}min {totalSeconds:00}sec"
            };

            gameTotalRunningTimeDialog.ShowDialog();
        }

        private void GameRunningTimeStatisticsMenuItemClick(object sender, RoutedEventArgs e)
        {
            ObservableCollection<GamePlayLogData> gamePlayLogDataCollection
                = GameLogDataGrid.DataContext as ObservableCollection<GamePlayLogData>;
            if (gamePlayLogDataCollection != null &&
                gamePlayLogDataCollection.Count > 0)
            {
                GameRunningTimeStatisticsDialog gameRunningTimeStatisticsDialog = new()
                {
                    Owner = this,
                    GamePlayLogDataCollection = gamePlayLogDataCollection
                };

                gameRunningTimeStatisticsDialog.ShowDialog();
            }
        }
    }
}
