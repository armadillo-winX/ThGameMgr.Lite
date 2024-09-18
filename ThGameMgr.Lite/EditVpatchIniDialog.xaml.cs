using System.Text;

namespace ThGameMgr.Lite
{
    /// <summary>
    /// EditVpatchIniDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class EditVpatchIniDialog : Window
    {
        public EditVpatchIniDialog()
        {
            InitializeComponent();
        }

        private string? _vsyncPatchIniFilePath;

        public string? VsyncPatchIniFilePath
        {
            get
            {
                return _vsyncPatchIniFilePath;
            }

            set
            {
                _vsyncPatchIniFilePath = value;
                if (!string.IsNullOrEmpty(value))
                {
                    GetVsyncPatchIniData(value);
                }
            }
        }

        private void GetVsyncPatchIniData(string vsyncPatchIniFilePath)
        {
            if (File.Exists(vsyncPatchIniFilePath))
            {
                try
                {
                    StreamReader streamReader = new(vsyncPatchIniFilePath, Encoding.UTF8);
                    string data = streamReader.ReadToEnd();
                    streamReader.Close();

                    EditorBox.Text = data;
                }
                catch (Exception ex)
                {
                    EditorBox.IsReadOnly = true;
                    EditorBox.Text = $"エラー\nvpatch.ini の読み込みに失敗しました。\n{ex.Message}";
                    SaveButton.IsEnabled = false;
                }
            }
            else
            {
                EditorBox.IsReadOnly = true;
                EditorBox.Text = "vpatch.ini が見つかりませんでした。";
                SaveButton.IsEnabled = false;
            }
        }

        private void SaveVsyncPatchIniData(string data)
        {
            if (!string.IsNullOrEmpty(this.VsyncPatchIniFilePath))
            {
                File.WriteAllText(this.VsyncPatchIniFilePath, data);
            }
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveVsyncPatchIniData(EditorBox.Text);
                MessageBox.Show(this, "保存しました。", "vpatch.ini エディタ",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"保存に失敗しました。\n{ex.Message}", "エラー",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                this.Close();
            }
        }
    }
}
