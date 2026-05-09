namespace Launcher.App.WinUI;

public partial class App : MauiWinUIApplication
{
    public App()
    {
        InitializeComponent();
        UnhandledException += (sender, e) =>
        {
            var errorPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "launcher_error.txt");
            System.IO.File.WriteAllText(errorPath, e.Message + "\n" + e.Exception?.ToString());
        };
    }

    protected override MauiApp CreateMauiApp()
    {
        try
        {
            return MauiProgram.CreateMauiApp();
        }
        catch (Exception ex)
        {
            var errorPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "launcher_error.txt");
            System.IO.File.WriteAllText(errorPath, ex.ToString());
            throw;
        }
    }
}
