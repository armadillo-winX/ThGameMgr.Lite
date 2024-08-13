using System.Collections.Generic;

namespace ThGameMgr.Lite.Game
{
    internal class GameIndex
    {
        private readonly static Dictionary<string, string> _gameNameDictionary = new()
        {
            { "Th06", "東方紅魔郷" },
            { "Th07", "東方妖々夢" },
            { "Th08", "東方永夜抄" },
            { "Th09", "東方花映塚" },
            { "Th095", "東方文花帖" },
            { "Th10", "東方風神録" },
            { "Th11", "東方地霊殿" },
            { "Th12", "東方星蓮船" },
            { "Th125", "ダブルスポイラー" },
            { "Th128", "妖精大戦争" },
            { "Th13", "東方神霊廟" },
            { "Th14", "東方輝針城" },
            { "Th143", "弾幕アマノジャク" },
            { "Th15", "東方紺珠伝" },
            { "Th16", "東方天空璋" },
            { "Th165", "秘封ナイトメアダイアリー" },
            { "Th17", "東方鬼形獣" },
            { "Th18", "東方虹龍洞" },
            { "Th185", "バレットフィリア達の闇市場" },
            { "Th19", "東方獣王園" }
        };

        private readonly static Dictionary<string, string> _gameSubTitleDictionary = new()
        {
            { "Th06", "～ the Embodiment of Scarlet Devil." },
            { "Th07", "～ Perfect Cherry Blossom." },
            { "Th08", "～ Imperishable Night." },
            { "Th09", "～ Phantasmagoria of Flower View." },
            { "Th095", "～ Shoot the Bullet." },
            { "Th10", "～ Mountain of Faith." },
            { "Th11", "～ Subterranean Animism." },
            { "Th12", "～ Undefined Fantastic Object." },
            { "Th125", "～ 東方文花帖" },
            { "Th128", "～ 東方三月精" },
            { "Th13", "～ Ten Desires." },
            { "Th14", "～ Double Dealing Character." },
            { "Th143", "～ Impossible Spell Card." },
            { "Th15", "～ Legacy of Lunatic Kingdom." },
            { "Th16", "～ Hidden Star in Four Seasons." },
            { "Th165", "～ Violet Detector." },
            { "Th17", "～ Wily Beast and Weakest Creature." },
            { "Th18", "～ Unconnected Marketeers." },
            { "Th185", "〜 100th Black Market." },
            { "Th19", "〜 Unfinished Dream of All Living Ghost." }
        };

        public static string GetGameName(string gameId)
        {
            if (_gameNameDictionary.TryGetValue(gameId, out string? gameName))
            {
                return gameName;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetGameSubtitle(string gameId)
        {
            if (_gameSubTitleDictionary.TryGetValue(gameId, out string? gameSubtitle))
            {
                return gameSubtitle;
            }
            else
            {
                return string.Empty;
            }
        }

        public static List<string> GetAllGamesList()
        {
            List<string> allGamesList = [];

            foreach (KeyValuePair<string, string> gameNameDictionaryKeyValuePair in _gameNameDictionary)
            {
                string gameId = gameNameDictionaryKeyValuePair.Key;

                allGamesList.Add(gameId);
            }

            return allGamesList;
        }

        public static List<string> GetEnabledGamesList()
        {
            List<string> enabledGamesList = [];

            //_gameNameDictionary から Key と Value のペアをひとつずつ取得
            foreach (KeyValuePair<string, string> gameNameDictionaryKeyValuePair in _gameNameDictionary)
            {
                string gameId = gameNameDictionaryKeyValuePair.Key;

                if (!string.IsNullOrEmpty(GameFile.GetGameFilePath(gameId)))
                {
                    enabledGamesList.Add(gameId);
                }
            }

            return enabledGamesList;
        }
    }
}
