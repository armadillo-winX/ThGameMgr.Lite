﻿using System.Collections.Generic;

namespace ThGameMgr.Lite.Game
{
    internal class GameSpecificConfig
    {
        private static Dictionary<string, bool?>? _autoResizerConfigDictionary;

        /// <summary>
        /// ウィンドウリサイザの自動起動オプションを返す(bool?値)
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public static bool? GetAutoResizerConfig(string gameId)
        {
            if (_autoResizerConfigDictionary == null)
            {
                return false;
            }
            else
            {
                if (_autoResizerConfigDictionary.TryGetValue(gameId, out bool? config))
                {
                    return config;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// ウィンドウリサイザの自動起動オプションを設定する
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="config"></param>
        public static void SetAutoResizerConfig(string gameId, bool? config)
        {
            if (_autoResizerConfigDictionary == null)
            {
                _autoResizerConfigDictionary = [];
                _autoResizerConfigDictionary.Add(gameId, config);
            }
            else
            {
                if (_autoResizerConfigDictionary.ContainsKey(gameId))
                {
                    _autoResizerConfigDictionary[gameId] = config;
                }
                else
                {
                    _autoResizerConfigDictionary.Add(gameId, config);
                }
            }
        }
    }
}
