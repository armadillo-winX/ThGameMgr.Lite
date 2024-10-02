using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThGameMgr.Lite
{
    /// <summary>
    /// GameRunningTimeStaticsDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class GameRunningTimeStatisticsDialog : Window
    {
        private ObservableCollection<GamePlayLogData>? _gamePlayLogDataCollection;

        public ObservableCollection<GamePlayLogData>? GamePlayLogDataCollection
        {
            get
            {
                return _gamePlayLogDataCollection;
            }

            set
            {
                _gamePlayLogDataCollection = value;
                if (value != null)
                {
                    StatisticsGamePlayLog(value);
                    ViewGameRunningTimeRanking(value);
                }
            }
        }

        public GameRunningTimeStatisticsDialog()
        {
            InitializeComponent();
        }

        private void StatisticsGamePlayLog(
            ObservableCollection<GamePlayLogData> gamePlayLogDataCollection)
        {
            List<string> allGamesList = GameIndex.GetAllGamesList();

            Dictionary<string, int> gameRunningTimeDictionary = [];

            foreach (string gameId in allGamesList)
            {
                gameRunningTimeDictionary.Add(gameId, 0);
            }

            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDataCollection)
            {
                try
                {
                    string gameId = gamePlayLogData.GameId;

                    string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                    int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                    int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);

                    if (!string.IsNullOrEmpty(gameId))
                        gameRunningTimeDictionary[gameId] += gameRunningTimeMin + gameRunningTimeSec;
                }
                catch (Exception)
                {

                }
            }

            ObservableCollection<GamePlayLogData> eachGameRunningTimeDataCollection = [];

            foreach (string gameId in allGamesList)
            {
                int gameRunningTime = gameRunningTimeDictionary[gameId];

                GamePlayLogData gamePlayLogData = new()
                {
                    GameId = gameId,
                    GameName = GameIndex.GetGameName(gameId),
                    GameRunningTime = $"{gameRunningTime / 60:00}:{gameRunningTime % 60:00}"
                };

                eachGameRunningTimeDataCollection.Add(gamePlayLogData);
            }

            EachGameRunningTimeGrid.AutoGenerateColumns = false;
            EachGameRunningTimeGrid.DataContext = eachGameRunningTimeDataCollection;
        }

        private void ViewGameRunningTimeRanking(
            ObservableCollection<GamePlayLogData> gamePlayLogDataCollection)
        {
            List<string> allGamesList = GameIndex.GetAllGamesList();

            Dictionary<string, int> gameRunningTimeDictionary = [];

            foreach (string gameId in allGamesList)
            {
                gameRunningTimeDictionary.Add(gameId, 0);
            }

            foreach (GamePlayLogData gamePlayLogData in gamePlayLogDataCollection)
            {
                try
                {
                    string gameId = gamePlayLogData.GameId;

                    string[] gameRunningTimeRecord = gamePlayLogData.GameRunningTime.Split(":");
                    int gameRunningTimeMin = int.Parse(gameRunningTimeRecord[0]) * 60;
                    int gameRunningTimeSec = int.Parse(gameRunningTimeRecord[1]);

                    if (!string.IsNullOrEmpty(gameId))
                        gameRunningTimeDictionary[gameId] += gameRunningTimeMin + gameRunningTimeSec;
                }
                catch (Exception)
                {

                }
            }

            List<int> gameRunningTimeList = [];

            foreach (KeyValuePair<string, int> gameRunningTimeDictionaryPair
                in gameRunningTimeDictionary)
            {
                int runningTime = gameRunningTimeDictionaryPair.Value;

                gameRunningTimeList.Add(runningTime);
            }

            if (gameRunningTimeList.Count > 0)
            {
                //昇順にソート
                gameRunningTimeList.Sort();
                //並べ替え(降順になる)
                gameRunningTimeList.Reverse();

                foreach (int gameRunningTime in gameRunningTimeList)
                {
                    string gameId = gameRunningTimeDictionary.FirstOrDefault(
                        x => x.Value.Equals(gameRunningTime)).Key;

                    //同一の実行時間の作品を区別するために逐一消す
                    gameRunningTimeDictionary.Remove(gameId);

                    RunnigTimeRankListBox.Items.Add(
                        $"{gameId}: {GameIndex.GetGameName(gameId)}   {gameRunningTime / 60:00}:{gameRunningTime % 60:00}\n");
                }
            }
        }
    }
}
