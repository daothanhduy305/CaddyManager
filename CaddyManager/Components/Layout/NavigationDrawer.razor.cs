using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Layout;

public partial class NavigationDrawer : ComponentBase
{
    /// <summary>
    /// List of navigation drawer items to be rendered in the UI
    /// </summary>
    private readonly List<DrawerItem> _drawerItems =
    [
        new()
        {
            Text = "Configurations",
            Icon = Icons.Custom.FileFormats.FileCode,
            Url = "/"
        },
        new()
        {
            Text = "Global Caddyfile",
            Icon = Icons.Material.Filled.Language,
            Url = "/caddyfile"
        },
    ];

    private bool _drawerOpen = false;
    
    internal void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }
}

/// <summary>
/// Model for a navigation drawer item
/// </summary>
internal struct DrawerItem
{
    /// <summary>
    /// Text to display
    /// </summary>
    public string Text { get; set; }
    
    /// <summary>
    /// Icon to display
    /// </summary>
    public string Icon { get; set; }
    
    /// <summary>
    /// Url to navigate to
    /// </summary>
    public string Url { get; set; }
}