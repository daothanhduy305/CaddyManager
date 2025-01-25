using MudBlazor;

namespace CaddyManager.Components.Layout;

public partial class MainLayout
{
    // To allow the menu button to control the drawer
    private NavigationDrawer _drawer = null!;

    // To control the dark mode
    private bool _isDarkMode;

    // Indicates if the component is initializing, hence would not render anything to not confuse the user
    private bool _isInitialing = true;

    // Used for the system preference for dark mode
    private MudThemeProvider _mudThemeProvider = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Get the system preference for dark mode
            _isDarkMode = await _mudThemeProvider.GetSystemPreference();
            await _mudThemeProvider.WatchSystemPreference(OnSystemPreferenceChanged);
            _isInitialing = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Method to handle the system preference change for dark mode
    /// </summary>
    /// <param name="newValue"></param>
    /// <returns></returns>
    private Task OnSystemPreferenceChanged(bool newValue)
    {
        _isDarkMode = newValue;
        StateHasChanged();
        return Task.CompletedTask;
    }
}
