using Microsoft.UI.Xaml;

// To learn more about WinUI, see: http://microsoft.com/winui
namespace MAUIStudy.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}