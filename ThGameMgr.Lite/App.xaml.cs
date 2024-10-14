using System.Configuration;
using System.Data;
using System.Windows;

namespace ThGameMgr.Lite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsAdmin()
        {
            //GetCurrentメソッドで現在のユーザーを取得
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            //System.Security.Principal.WindowsPrincipalを作成このオブジェクトは権限の確認ができる
            System.Security.Principal.WindowsPrincipal principal = new(identity);

            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }

}
