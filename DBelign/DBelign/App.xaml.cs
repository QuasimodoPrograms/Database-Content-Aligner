using System;
using System.Threading;
using System.Windows;

namespace DBelign
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create a new splash screen from a specified image
            SplashScreen splash = new SplashScreen("/Images/splash.png");
            splash.Show(false);
            Thread.Sleep(500);

            splash.Close(new TimeSpan(0, 0, 1));
        }
    }
}
