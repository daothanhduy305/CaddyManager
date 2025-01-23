using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.ReverseProxies;

public partial class ReverseProxiesPage : ComponentBase
{
    private List<string> _availableCaddyConfigurations = [];
    private IReadOnlyCollection<string> _selectedCaddyConfigurations = [];

    protected override Task OnInitializedAsync()
    {
        _availableCaddyConfigurations = CaddyService.GetExistingCaddyConfigurations();
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Method to help open the dialog to create a new reverse proxy configuration
    /// </summary>
    /// <returns></returns>
    private Task NewReverseProxy()
    {
        return DialogService.ShowAsync<CaddyfileEditor.CaddyfileEditor>("New configuration",
            options: new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
            }, parameters: new DialogParameters
            {
                { "FileName", string.Empty }
            });
    }
}