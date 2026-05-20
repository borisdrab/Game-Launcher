namespace Launcher.App.Views;

public partial class NavigationBarView : ContentView
{
    public static readonly BindableProperty ActiveTabProperty =
        BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(NavigationBarView),
            defaultValue: "Obchod", propertyChanged: OnActiveTabChanged);

    public string ActiveTab
    {
        get => (string)GetValue(ActiveTabProperty);
        set => SetValue(ActiveTabProperty, value);
    }

    public NavigationBarView()
    {
        InitializeComponent();
    }

    private static void OnActiveTabChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is NavigationBarView navBar)
            navBar.UpdateTabStyles();
    }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        UpdateTabStyles();
    }

    private void UpdateTabStyles()
    {
        SetTabStyle(ObchodTab, ActiveTab == "Obchod");
        SetTabStyle(KnihivnaTab, ActiveTab == "Knihovna");
        SetTabStyle(SocialsTab, ActiveTab == "Socials");
        SetTabStyle(UserTab, ActiveTab == "Users");
    }

    private static void SetTabStyle(Border tab, bool isActive)
    {
        if (isActive)
        {
            tab.Stroke = Color.FromArgb("#CC3333");
            tab.StrokeThickness = 2;
            tab.BackgroundColor = Color.FromArgb("#4A2020");
        }
        else
        {
            tab.Stroke = Color.FromArgb("#666666");
            tab.StrokeThickness = 1;
            tab.BackgroundColor = Color.FromArgb("#3D3333");
        }
    }

    private async void OnObchodTapped(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//games");

    private async void OnKnihovnaTapped(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//library");

    private async void OnSocialsTapped(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//socials");

    private async void OnUserTapped(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//users");
}
