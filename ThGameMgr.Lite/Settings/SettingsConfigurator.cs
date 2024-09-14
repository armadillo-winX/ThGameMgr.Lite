using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ThGameMgr.Lite.Settings
{
    internal class SettingsConfigurator
    {
        public static void SaveGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            XmlDocument gamePathSettingsXml = new();

            XmlNode docNode = gamePathSettingsXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gamePathSettingsXml.AppendChild(docNode);

            XmlNode rootNode = gamePathSettingsXml.CreateElement("GamePathSettings");
            _ = gamePathSettingsXml.AppendChild(rootNode);

            foreach (string gameId in allGamesList)
            {
                string gameFilePath = GameFile.GetGameFilePath(gameId);
                XmlElement gamePathConfigNode = gamePathSettingsXml.CreateElement(gameId);
                gamePathConfigNode.InnerText = gameFilePath ?? string.Empty; //nullチェック

                _ = rootNode.AppendChild(gamePathConfigNode);
            }

#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            gamePathSettingsXml.Save(gamePathSettingsFile);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
        }

        public static void ConfigureGamePathSettings()
        {
            string? gamePathSettingsFile = PathInfo.GamePathSettingsFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gamePathSettingsFile) && File.Exists(gamePathSettingsFile))
            {
                XmlDocument gamePathSettingsXml = new();
                gamePathSettingsXml.Load(gamePathSettingsFile);

                XmlElement rootNode = gamePathSettingsXml.DocumentElement;

                foreach (string gameId in allGamesList)
                {
                    XmlNode gamePathConfigNode = rootNode.SelectSingleNode(gameId);
                    if (gamePathConfigNode != null)
                    {
                        GameFile.SetGameFilePath(gameId, gamePathConfigNode.InnerText);
                    }
                    else
                    {
                        GameFile.SetGameFilePath(gameId, string.Empty);
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameFile.SetGameFilePath(gameId, string.Empty);
                }
            }
        }

        public static void SaveGameSpecificConfig()
        {
            string? gameSpecificConfigFile = PathInfo.GameSpecificConfigFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            XmlDocument gameSpecificConfigXml = new();

            XmlNode docNode = gameSpecificConfigXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = gameSpecificConfigXml.AppendChild(docNode);

            XmlNode rootNode = gameSpecificConfigXml.CreateElement("GameSpecificConfig");
            _ = gameSpecificConfigXml.AppendChild(rootNode);

            XmlNode autoResizerConfigRootNode =
                gameSpecificConfigXml.CreateElement("AutoResizerConfig");
            _ = rootNode.AppendChild(autoResizerConfigRootNode);

            foreach (string gameId in allGamesList)
            {
                bool? config = GameSpecificConfig.GetAutoResizerConfig(gameId);
                XmlElement autoResizerConfigNode = gameSpecificConfigXml.CreateElement(gameId);
                autoResizerConfigNode.InnerText = (config == true).ToString(); //nullチェック

                _ = autoResizerConfigRootNode.AppendChild(autoResizerConfigNode);
            }

#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            gameSpecificConfigXml.Save(gameSpecificConfigFile);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
        }

        public static void ConfigureGameSpecificConfig()
        {
            string? gameSpecificConfigFile = PathInfo.GameSpecificConfigFile;

            List<string> allGamesList = GameIndex.GetAllGamesList();

            if (!string.IsNullOrEmpty(gameSpecificConfigFile) && File.Exists(gameSpecificConfigFile))
            {
                XmlDocument gameSpecificConfigXml = new();
                gameSpecificConfigXml.Load(gameSpecificConfigFile);

                XmlElement rootNode = gameSpecificConfigXml.DocumentElement;

                XmlNode autoResizerConfigRootNode
                    = rootNode.SelectSingleNode("AutoResizerConfig");

                foreach (string gameId in allGamesList)
                {
                    XmlNode gamePathConfigNode = autoResizerConfigRootNode.SelectSingleNode(gameId);
                    if (gamePathConfigNode != null)
                    {
                        GameSpecificConfig.SetAutoResizerConfig(gameId, gamePathConfigNode.InnerText.ToLower() == "true");
                    }
                    else
                    {
                        GameSpecificConfig.SetAutoResizerConfig(gameId, false);
                    }
                }
            }
            else
            {
                foreach (string gameId in allGamesList)
                {
                    GameSpecificConfig.SetAutoResizerConfig(gameId, false);
                }
            }
        }

        public static void SaveMainWindowSettings(MainWindowSettings mainWindowSettings)
        {
            string? mainWindowSettingsFile = PathInfo.MainWindowSettingsFile;

            XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            FileStream fileStream = new(mainWindowSettingsFile, FileMode.Create);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
            mainWindowSettingsSerializer.Serialize(fileStream, mainWindowSettings);
            fileStream.Close();
        }

        public static MainWindowSettings ConfigureMainWindowSettings()
        {
            string? mainWindowSettingsFile = PathInfo.MainWindowSettingsFile;

            MainWindowSettings? mainWindowSettings = new();

            if (!string.IsNullOrEmpty(mainWindowSettingsFile) &&
                File.Exists(mainWindowSettingsFile))
            {
                XmlSerializer mainWindowSettingsSerializer = new(typeof(MainWindowSettings));
                FileStream fileStream = new(mainWindowSettingsFile, FileMode.Open);

                mainWindowSettings = (MainWindowSettings)mainWindowSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                mainWindowSettings.MainWindowLocationX = 100;
                mainWindowSettings.MainWindowLocationY = 100;
                mainWindowSettings.SelectedGameId = string.Empty;
                mainWindowSettings.MainWindowAlwaysOnTop = false;
            }

            if (mainWindowSettings == null)
            {
                mainWindowSettings.MainWindowLocationX = 100;
                mainWindowSettings.MainWindowLocationY = 100;
                mainWindowSettings.SelectedGameId = string.Empty;
                mainWindowSettings.MainWindowAlwaysOnTop = false;
            }

            return mainWindowSettings;
        }

        public static void SaveResizerFrameWindowSettings(ResizerFrameWindowSettings resizerFrameWindowSettings)
        {
            string? resizerFrameWindowSettingsFile = PathInfo.ResizerFrameSettingsFile;

            XmlSerializer resizerFrameWindowSettingsSerializer = new(typeof(ResizerFrameWindowSettings));
#pragma warning disable CS8604 // Null 参照引数の可能性があります。
            FileStream fileStream = new(resizerFrameWindowSettingsFile, FileMode.Open);
#pragma warning restore CS8604 // Null 参照引数の可能性があります。
            resizerFrameWindowSettingsSerializer.Serialize(fileStream, resizerFrameWindowSettings);
            fileStream.Close();
        }

        public static ResizerFrameWindowSettings ConfigureResizerFrameWindowSettings()
        {
            string? resizerFrameWindowSettingsFile = PathInfo.ResizerFrameSettingsFile;

            ResizerFrameWindowSettings resizerFrameWindowSettings = new();

            if (!string.IsNullOrEmpty(resizerFrameWindowSettingsFile) &&
                File.Exists(resizerFrameWindowSettingsFile))
            {
                XmlSerializer resizerFrameWindowSettingsSerializer = new(typeof(ResizerFrameWindowSettings));
                FileStream fileStream = new(resizerFrameWindowSettingsFile, FileMode.Open);

                resizerFrameWindowSettings 
                    = (ResizerFrameWindowSettings)resizerFrameWindowSettingsSerializer.Deserialize(fileStream);
                fileStream.Close();
            }
            else
            {
                resizerFrameWindowSettings.FixAspectRate = true;
                resizerFrameWindowSettings.AutoClose = false;
            }

            if (resizerFrameWindowSettings == null)
            {
                resizerFrameWindowSettings.FixAspectRate = true;
                resizerFrameWindowSettings.AutoClose = false;
            }

            return resizerFrameWindowSettings;
        }
    }
}
