global using System;
global using System.Diagnostics;
global using System.IO;
global using System.Windows;

global using ThGameMgr.Lite.Exceptions;
global using ThGameMgr.Lite.Game;
global using ThGameMgr.Lite.Settings;

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ThGameMgr.Lite
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _gameId;
        private string? _gameName;

        private BackgroundWorker? _gameEndWaitingModeWorker = null;
        private DispatcherTimer? _gameControlTimer = null;
        private ResizerFrameWindow? _resizerFrameWindow = null;

        private string? GameId
        {
            get
            {
                return _gameId;
            }

            set
            {
                _gameId = value;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    GameIdBlock.Text = value;
                    AutoStartWindowResizerCheckBox.IsChecked = GameSpecificConfig.GetAutoResizerConfig(value);
                    string gameName = GameIndex.GetGameName(value);
                    this.GameName = gameName;
                }
                else
                {
                    GameIdBlock.Text = "Th00";
                    this.GameName = string.Empty;
                }
            }
        }

        private string? GameName
        {
            get
            {
                return _gameName;
            }

            set
            {
                _gameName = value;

                if (!string.IsNullOrEmpty(value))
                {
                    GameNameBlock.Text = value;
                }
                else
                {
                    GameNameBlock.Text = "[ゲーム未選択]";
                }
            }
        }

        private Process GameProcess { get; set; }

        private bool IsGameEndWaitModeEnabled { get; set; }

        private DateTime GameStartTime { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Title = VersionInfo.AppName;
            this.GameId = string.Empty;
            this.IsGameEndWaitModeEnabled = false;
            this.GameProcess = new();
            ReimuImageView.Visibility = Visibility.Hidden;

            EnableLimitationMode(false);

            AppStatusItem.Content = "準備完了";

            if (App.IsAdmin())
                this.Title += " (管理者)";

            //各種設定構成処理
            //メインウィンドウの設定構成処理はすべての設定が構成された後に行う
            if (Directory.Exists(PathInfo.SettingsDirectory))
            {
                if (File.Exists(PathInfo.GamePathSettingsFile))
                {
                    try
                    {
                        SettingsConfigurator.ConfigureGamePathSettings();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ゲームパス設定の構成処理に失敗しました。\n{ex.Message}", "エラー",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("ゲームのパスを設定してください。", VersionInfo.AppName,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    GamePathSettingsDialog gamePathSettingsDialog = new();
                    gamePathSettingsDialog.ShowDialog();
                    //this.GameId = GameIndex.GetFirstEnableGame();
                }

                try
                {
                    SettingsConfigurator.ConfigureGameSpecificConfig();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ゲーム別構成の復元に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }

                ConfigureMainWindowSettings();
            }
            else
            {
                try
                {
                    if (!string.IsNullOrEmpty(PathInfo.SettingsDirectory))
                        Directory.CreateDirectory(PathInfo.SettingsDirectory);

                    MessageBox.Show("ゲームのパスを設定してください。", VersionInfo.AppName,
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    GamePathSettingsDialog gamePathSettingsDialog = new();
                    gamePathSettingsDialog.ShowDialog();
                    this.GameId = GameIndex.GetFirstEnableGame();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"設定ファイル保存ディレクトリの生成に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            SetGameSelectionMenu();
        }

        private void SetGameSelectionMenu()
        {
            SelectGameButtonContextMenu.Items.Clear();

            List<string> enabledGamesList = GameIndex.GetEnabledGamesList();

            if (enabledGamesList.Count > 0)
            {
                foreach (string gameId in enabledGamesList)
                {
                    string gameName = GameIndex.GetGameName(gameId);

                    MenuItem gameMenuItem = new()
                    {
                        Header = $"{gameId}: {gameName}",
                        Uid = gameId
                    };

                    gameMenuItem.Click += new RoutedEventHandler(GameSelectionMenuItemClick);

                    SelectGameButtonContextMenu.Items.Add(gameMenuItem);
                }
            }

            Separator separator = new();
            SelectGameButtonContextMenu.Items.Add(separator);

            MenuItem gamePathSettingsMenuItem = new()
            {
                Header = "ゲームのパスを設定する",
                Icon =
                new Image
                {
                    Source = new BitmapImage(
                        new Uri("pack://application:,,,/ThGameMgr.Lite;component/Images/Settings.png")
                        )
                }
            };
            gamePathSettingsMenuItem.Click += new RoutedEventHandler(SetGamePathMenuItemClick);
            SelectGameButtonContextMenu.Items.Add(gamePathSettingsMenuItem);
        }

        private void ConfigureMainWindowSettings()
        {
            try
            {
                MainWindowSettings mainWindowSettings = SettingsConfigurator.ConfigureMainWindowSettings();

                this.Left = mainWindowSettings.MainWindowLocationX;
                this.Top = mainWindowSettings.MainWindowLocationY;
                this.GameId = mainWindowSettings.SelectedGameId;
                this.Topmost = mainWindowSettings.MainWindowAlwaysOnTop;
                AlwaysOnTopMenuItem.IsChecked = mainWindowSettings.MainWindowAlwaysOnTop;
                ReimuMenuItem.IsChecked = mainWindowSettings.ShowReimu;
                if (mainWindowSettings.ShowReimu)
                {
                    ReimuImageView.Visibility = Visibility.Visible;
                }
                else
                {
                    ReimuImageView.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"メインウィンドウ設定の構成に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EnableLimitationMode(bool enabled)
        {
            StartGameMenuItem.IsEnabled = !enabled;
            StartGameWithVpatchMenuItem.IsEnabled = !enabled;
            StartGameWithThpracMenuItem.IsEnabled = !enabled;
            StartCustomProgramMenuItem.IsEnabled = !enabled;
            EditVpatchIniMenuItem.IsEnabled = !enabled;

            SelectGameButton.IsEnabled = !enabled;
            StartGameButton.IsEnabled = !enabled;
            StartGameWithVpatchButton.IsEnabled = !enabled;
            StartGameWithThpracButton.IsEnabled = !enabled;
            StartCustomProgramButton.IsEnabled = !enabled;

            StartWindowResizerMenuItem.IsEnabled = enabled;
        }

        private void StartGameEndWaitingMode(Process gameProcess)
        {
            this.IsGameEndWaitModeEnabled = true;
            this.GameProcess = gameProcess;
            this.GameStartTime = gameProcess.StartTime;

            int gameProcessId = gameProcess.Id;

            EnableLimitationMode(true);

            AppStatusItem.Content
                = !string.IsNullOrEmpty(this.GameId) ? $"{GameIndex.GetGameName(this.GameId)}を実行中..." : "Unknownを実行中...";

            _gameEndWaitingModeWorker = new BackgroundWorker();
            _gameEndWaitingModeWorker.DoWork += new DoWorkEventHandler(WorkerDoWork);
            _gameEndWaitingModeWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerRunningComplete);
            _gameEndWaitingModeWorker.RunWorkerAsync(gameProcess);

            _gameControlTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            _gameControlTimer.Tick += (e, s) =>
            {
                TimeSpan time = DateTime.Now - this.GameStartTime;
                GameRunningTimeBlock.Text = time.ToString(@"mm\m\i\nss\s\e\c");

                if (AutoStartWindowResizerCheckBox.IsChecked == true &&
                _resizerFrameWindow == null)
                {
                    try
                    {
                        //タイマー処理の度にプロセスIDからプロセスを取得する
                        gameProcess = Process.GetProcessById(gameProcessId);

                        GameWindowSizes gameWindowSizes = GameWindowHandler.GetWindowSizes(gameProcess.MainWindowHandle);

                        if (gameWindowSizes.Width > 500)
                        {
                            _resizerFrameWindow = new()
                            {
                                GameWindow = gameProcess.MainWindowHandle
                            };

                            _resizerFrameWindow.Show();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            };

            _gameControlTimer.Start();
        }

        private void WorkerDoWork(object? sender, DoWorkEventArgs e)
        {
            Process gameProcess = (Process)e.Argument;
            gameProcess.WaitForExit();
        }

        private void WorkerRunningComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.IsGameEndWaitModeEnabled = false;
            this.GameProcess.Dispose();
            _gameControlTimer?.Stop();

            DateTime gameEndTime = DateTime.Now;

            TimeSpan runningTimeSpan = gameEndTime - this.GameStartTime;

            GamePlayLogData gamePlayLogData = new()
            {
                GameId = this.GameId,
                GameName = this.GameName,
                GameStartTime = this.GameStartTime.ToString("yyyy/MM/dd HH:mm:ss"),
                GameEndTime = gameEndTime.ToString("yyyy/MM/dd HH:mm:ss"),
                GameRunningTime = runningTimeSpan.ToString("mm\\:ss")
            };

            if (_resizerFrameWindow != null && _resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow.Close();
            }
            _resizerFrameWindow = null;

            try
            {
                GamePlayLogRecorder.SaveGamePlayLog(gamePlayLogData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"ゲーム実行履歴の保存に失敗しました。\n{ex.Message}",
                                "エラー",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }

            EnableLimitationMode(false);
            AppStatusItem.Content = "準備完了";
        }

        private void GameSelectionMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                string gameId = ((MenuItem)sender).Uid;
                if (!string.IsNullOrEmpty(gameId))
                {
                    this.GameId = gameId;
                }
                else
                {
                    MessageBox.Show(this, "不正な選択です。", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(this, "不正な選択です。",
                    "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetGamePathMenuItemClick(object sender, RoutedEventArgs e)
        {
            GamePathSettingsDialog gamePathSettingsDialog = new()
            {
                Owner = this
            };
            gamePathSettingsDialog.ShowDialog();

            SetGameSelectionMenu();
        }

        private void ExitMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SelectGameButtonClick(object sender, RoutedEventArgs e)
        {
            if (!SelectGameButtonContextMenu.IsOpen)
            {
                SelectGameButtonContextMenu.PlacementTarget = SelectGameButton;
                SelectGameButtonContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                SelectGameButtonContextMenu.IsOpen = true;
            }
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            try
            {
                SettingsConfigurator.SaveGamePathSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲームパス設定の保存を失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                SettingsConfigurator.SaveGameSpecificConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ゲーム別構成の保存に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                MainWindowSettings applicationSettings = new()
                {
                    MainWindowLocationX = this.Left,
                    MainWindowLocationY = this.Top,
                    SelectedGameId = this.GameId,
                    MainWindowAlwaysOnTop = AlwaysOnTopMenuItem.IsChecked,
                    ShowReimu = ReimuMenuItem.IsChecked
                };

                SettingsConfigurator.SaveMainWindowSettings(applicationSettings);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"メインウィンドウ設定の保存を失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void StartGameMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableLimitationMode(true);
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcess(gameId)
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableLimitationMode(false);
                }
            }
        }

        private async void StartGameWithVpatchMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableLimitationMode(true);
                try
                {
                    Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, "vpatch.exe")
                                );
                    StartGameEndWaitingMode(gameProcess);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableLimitationMode(false);
                }
            }
        }

        private async void StartGameWithThpracMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                EnableLimitationMode(true);
                try
                {
                    List<string> thpracFiles = GameFile.GetThpracFiles(gameId);
                    if (thpracFiles.Count == 0)
                    {
                        MessageBox.Show(this, "利用可能な thprac 実行ファイルが存在しません。", "thprac の適用",
                            MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (thpracFiles.Count == 1)
                    {
                        Process gameProcess
                        = await Task.Run(()
                                => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, thpracFiles[0])
                                );
                        StartGameEndWaitingMode(gameProcess);
                    }
                    else
                    {
                        SelectThpracDialog selectThpracDialog = new()
                        {
                            ThpracFiles = thpracFiles
                        };

                        if (selectThpracDialog.ShowDialog() == true)
                        {
                            string? thpracFile = selectThpracDialog.SelectedThpracFile;
                            if (!string.IsNullOrEmpty(thpracFile))
                            {
                                Process gameProcess
                                = await Task.Run(()
                                        => GameProcessHandler.StartGameProcessWithApplyingTool(gameId, thpracFile)
                                        );
                                StartGameEndWaitingMode(gameProcess);
                            }
                            else
                            {
                                MessageBox.Show(this, "選択された thprac 実行ファイルが不正です。", "エラー",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                EnableLimitationMode(false);
                            }
                        }
                        else
                        {
                            EnableLimitationMode(false);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"ゲームの起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    EnableLimitationMode(false);
                }
            }
        }

        private void StartCustomProgramMenuItemClick(object sender, RoutedEventArgs e)
        {
            string gameId = this.GameId;
            if (!string.IsNullOrEmpty(gameId))
            {
                try
                {
                    GameProcessHandler.StartCustomProgramProcess(gameId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"環境カスタムプログラム(custom.exe)の起動に失敗しました。\n{ex.Message}", "エラー",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void AlwaysOnTopMenuItemClick(object sender, RoutedEventArgs e)
        {
            this.Topmost = AlwaysOnTopMenuItem.IsChecked;
        }

        private void AboutMenuItemClick(object sender, RoutedEventArgs e)
        {
            AboutDialog aboutDialog = new()
            {
                Owner = this
            };
            aboutDialog.ShowDialog();
        }

        private void StartWindowResizerMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (_resizerFrameWindow == null || !_resizerFrameWindow.IsLoaded)
            {
                _resizerFrameWindow = new()
                {
                    GameWindow = this.GameProcess.MainWindowHandle
                };

                _resizerFrameWindow.Show();
            }
        }

        private void AutoStartWindowResizerCheckBoxClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GameId))
            {
                bool config = AutoStartWindowResizerCheckBox.IsChecked == true;
                GameSpecificConfig.SetAutoResizerConfig(this.GameId, config);
            }
        }

        private void ViewGamePlayLogMenuItemClick(object sender, RoutedEventArgs e)
        {
            GamePlayLogDialog gamePlayLogDialog = new()
            {
                Owner = this
            };

            gamePlayLogDialog.ShowDialog();
        }

        private void OpenReadMeMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PathInfo.ReadMeFile != null)
                {
                    ProcessStartInfo processStartInfo = new()
                    {
                        FileName = PathInfo.ReadMeFile,
                        UseShellExecute = true
                    };

                    Process.Start(processStartInfo);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendFeedbackMenuItemClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo processStartInfo = new()
                {
                    FileName
                    = "https://forms.gle/6aHvWavL5N6Rvnnu9",
                    UseShellExecute = true
                };

                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditVpatchIniMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(this.GameId))
            {
                string gameFilePath = GameFile.GetGameFilePath(this.GameId);
                string gameDirectoryPath = Path.GetDirectoryName(gameFilePath);

                string vpatchIniFilePath = $"{gameDirectoryPath}\\vpatch.ini";

                if (File.Exists(vpatchIniFilePath))
                {
                    EditVpatchIniDialog editVpatchIniDialog = new()
                    {
                        Owner = this,
                        VsyncPatchIniFilePath = vpatchIniFilePath
                    };

                    editVpatchIniDialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show(this, "vpatch.ini が見つかりませんでした。", "vpatch.ini を編集",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ReimuMenuItemClick(object sender, RoutedEventArgs e)
        {
            if (ReimuMenuItem.IsChecked)
            {
                ReimuImageView.Visibility = Visibility.Visible;
            }
            else
            {
                ReimuImageView.Visibility= Visibility.Hidden;
            }
        }
    }
}