using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using QP = QP_Helpers.QP_Helpers;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for Window_Buy.xaml
    /// </summary>
    public partial class Window_Buy : Window
    {
        public Window_Buy()
        {
            InitializeComponent();
        }

        private void DragWithHeader(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void btn_Register_Click(object sender, RoutedEventArgs e)
        {
            if (QP.IsLicensed(tb_Password.Text, "DBelign.LicenseKeys.txt"))
            {
                QP.SetRegister(@"HKEY_CURRENT_USER\Software\Quasimodo Programs\DBelign", "KeY", tb_Password.Text);
                QP._isLicensed = true;
                MessageBox.Show("Successful registration.");
                Close();
            }
            else
                MessageBox.Show("The key is not licensed.");
        }

        private void btn_Buy_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.youtube.com/c/QuasimodoPrograms?sub_confirmation=1");
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }




    }
}
