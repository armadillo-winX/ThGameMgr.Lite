using System.Collections.ObjectModel;
using System.Xml;

namespace ThGameMgr.Lite.Game
{
    internal class GamePlayLogRecorder
    {
        public static void SaveGamePlayLog(GamePlayLogData gamePlayLogData)
        {
            string gamePlayLogFile = PathInfo.GamePlayLogRecordFile;

            if (!File.Exists(gamePlayLogFile))
            {
                CreateGamePlayLogFile();
            }

            XmlDocument gamePlayLogXml = new();
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            gamePlayLogXml.Load(gamePlayLogFile);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
            XmlElement rootNode = gamePlayLogXml.DocumentElement;

            XmlElement gamePlayLog = gamePlayLogXml.CreateElement("GamePlayLog");

            //ノードをRootノードに追加
            _ = rootNode.AppendChild(gamePlayLog);

            XmlElement gameId = gamePlayLogXml.CreateElement("GameId");
            _ = gameId.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameId));
            _ = gamePlayLog.AppendChild(gameId);

            XmlElement gameName = gamePlayLogXml.CreateElement("GameName");
            _ = gameName.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameName));
            _ = gamePlayLog.AppendChild(gameName);

            XmlElement gameStartTime = gamePlayLogXml.CreateElement("GameStartTime");
            _ = gameStartTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameStartTime));
            _ = gamePlayLog.AppendChild(gameStartTime);

            XmlElement gameEndTime = gamePlayLogXml.CreateElement("GameEndTime");
            _ = gameEndTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameEndTime));
            _ = gamePlayLog.AppendChild(gameEndTime);

            XmlElement gameRunningTime = gamePlayLogXml.CreateElement("GameRunningTime");
            _ = gameRunningTime.AppendChild(gamePlayLogXml.CreateTextNode(gamePlayLogData.GameRunningTime));
            _ = gamePlayLog.AppendChild(gameRunningTime);

            gamePlayLogXml.Save(gamePlayLogFile);
        }

        public static ObservableCollection<GamePlayLogData> GetGamePlayLogDatas()
        {
            string gamePlayLogFile = PathInfo.GamePlayLogRecordFile;

            ObservableCollection<GamePlayLogData> gamePlayLogDatas = new();

            XmlDocument gameLogDataXml = new();
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            gameLogDataXml.Load(gamePlayLogFile);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
            XmlNodeList allGameLogs = gameLogDataXml.SelectNodes("GamePlayLogData/GamePlayLog");
            if (allGameLogs.Count != 0)
            {
                foreach (XmlNode gameLog in allGameLogs)
                {
                    GamePlayLogData gamePlayLogData = new()
                    {
                        GameId = gameLog.SelectSingleNode("GameId").InnerText,
                        GameName = gameLog.SelectSingleNode("GameName").InnerText,
                        GameStartTime = gameLog.SelectSingleNode("GameStartTime").InnerText,
                        GameEndTime = gameLog.SelectSingleNode("GameEndTime").InnerText,
                        GameRunningTime = gameLog.SelectSingleNode("GameRunningTime").InnerText
                    };

                    gamePlayLogDatas.Add(gamePlayLogData);
                }
            }

            return gamePlayLogDatas;
        }

        public static void CreateGamePlayLogFile()
        {
            string gamePlayLogFile = PathInfo.GamePlayLogRecordFile;

            XmlDocument gamePlayLogXml = new();
            XmlNode docNode = gamePlayLogXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gamePlayLogXml.AppendChild(docNode);

            XmlNode rootNode = gamePlayLogXml.CreateElement("GamePlayLogData");
            _ = gamePlayLogXml.AppendChild(rootNode);

#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            gamePlayLogXml.Save(gamePlayLogFile);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
        }
    }
}
