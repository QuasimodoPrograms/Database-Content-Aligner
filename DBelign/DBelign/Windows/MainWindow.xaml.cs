using System.Windows;
using QP = QP_Helpers.QP_Helpers;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string fullRegistryPath = @"HKEY_CURRENT_USER\Software\Quasimodo Programs\DBelign";
            string registryValueName = "KeY";

            string key = QP.GetRegister(fullRegistryPath, registryValueName);

            if (key != null && QP.IsLicensed(key, "DBelign.LicenseKeys.txt"))
                QP._isLicensed = true;
            else
            {
                QP._isLicensed = false;

                Window_Buy win = new Window_Buy();

                win.ShowDialog();
            }

            InitializeComponent();
        }
    }
}
