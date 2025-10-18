using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace CrossNGram.MAUI;

public partial class App : Application
{
    private static string LogFilePath => Path.Combine(Path.GetTempPath(), "CrossNGram.MAUI.launch.log.txt");

    public App()
    {
        InitializeComponent();
        AppendLog("App constructed");
        RegisterGlobalExceptionHandlers();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        AppendLog("CreateWindow invoked");
        try
        {
            var window = new Window(new NavigationPage(new MainPage()))
            {
                Title = "CrossNGram"
            };

            AppendLog("CreateWindow succeeded");
            return window;
        }
        catch (Exception ex)
        {
            AppendLog($"CreateWindow failed: {ex}");
            var fallback = new ContentPage
            {
                Content = new ScrollView
                {
                    Content = new Label
                    {
                        Text = "Startup failed:\n" + ex,
                        FontSize = 14
                    }
                }
            };

            return new Window(fallback) { Title = "CrossNGram (Fallback)" };
        }
    }

    private static void RegisterGlobalExceptionHandlers()
    {
        try
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
                AppendLog($"Unhandled exception: {e.ExceptionObject}");

            TaskScheduler.UnobservedTaskException += (_, e) =>
            {
                AppendLog($"Unobserved task exception: {e.Exception}");
                e.SetObserved();
            };
        }
        catch (Exception ex)
        {
            AppendLog($"Failed to register global handlers: {ex}");
        }
    }

    private static void AppendLog(string message)
    {
        try
        {
            File.AppendAllText(LogFilePath, $"[{DateTime.Now:u}] {message}{Environment.NewLine}");
        }
        catch
        {
            // Logging failures should not block startup.
        }
    }
}
