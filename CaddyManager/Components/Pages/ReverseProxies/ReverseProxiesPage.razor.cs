namespace CaddyManager.Components.Pages.ReverseProxies;

public partial class ReverseProxiesPage
{
    private List<string> _availableCaddyConfigurations = [];

    protected override Task OnInitializedAsync()
    {
        _availableCaddyConfigurations = CaddyService.GetExistingCaddyConfigurations();
        return base.OnInitializedAsync();
    }
}