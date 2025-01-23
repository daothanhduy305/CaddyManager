using Microsoft.AspNetCore.Components;

namespace CaddyManager.Components.Layout;

public partial class NavigationDrawer : ComponentBase
{
    private bool _drawerOpen = false;
    
    internal void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }
}