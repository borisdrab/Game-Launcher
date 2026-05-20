namespace Launcher.App.Services;

public class AlertService : IAlertService
{
    private static Page? CurrentPage
        => Application.Current?.Windows.FirstOrDefault()?.Page;

    public async Task DisplayAsync(string title, string message)
    {
        if (CurrentPage is { } page)
            await page.DisplayAlertAsync(title, message, "OK");
    }

    public async Task<bool> ConfirmAsync(string title, string message, string accept, string cancel)
    {
        if (CurrentPage is { } page)
            return await page.DisplayAlertAsync(title, message, accept, cancel);
        return false;
    }
}
