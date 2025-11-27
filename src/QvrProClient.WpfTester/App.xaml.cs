using System.Windows;
using QvrProClient.WpfTester.ViewModels;
using QvrProClient.WpfTester.Views;

namespace QvrProClient.WpfTester;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var mainViewModel = new MainViewModel();
        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };

        mainWindow.Show();
    }
}
