using System.Collections.Generic;

namespace ThGameMgr.Lite.Game
{
    internal class GameFile
    {
        private static Dictionary<string, string>? _gameFilesDictionary;

        public static string? GetGameFilePath(string gameId)
        {
            if (_gameFilesDictionary == null)
            {
                return string.Empty;
            }
            else
            {
                if (_gameFilesDictionary.TryGetValue(gameId, out string? gameFilePath))
                {
                    return gameFilePath;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public static void SetGameFilePath(string gameId, string gameFilePath)
        {
            if (_gameFilesDictionary == null)
            {
                _gameFilesDictionary = [];
                _gameFilesDictionary.Add(gameId, gameFilePath);
            }
            else
            {
                if (_gameFilesDictionary.ContainsKey(gameId))
                {
                    _gameFilesDictionary[gameId] = gameFilePath;
                }
                else
                {
                    _gameFilesDictionary.Add(gameId, gameFilePath);
                }
            }
        }

        public static List<string> GetThpracFiles(string gameId)
        {
            string gameFile = GetGameFilePath(gameId);
            string gameDirectory = Path.GetDirectoryName(gameFile);
            if (!string.IsNullOrEmpty(gameDirectory))
            {
                List<string> thpracFiles = new();

                foreach (string thpracFile in 
                    Directory.GetFiles(gameDirectory, "thprac*.exe", SearchOption.TopDirectoryOnly))
                {
                    string thpracFileName = Path.GetFileName(thpracFile);
                    thpracFiles.Add(thpracFileName);
                }

                return thpracFiles;
            }
            else
            {
                throw new DirectoryNotFoundException("ゲームがインストールされているフォルダが見つかりませんでした。");
            }
        }
    }
}
