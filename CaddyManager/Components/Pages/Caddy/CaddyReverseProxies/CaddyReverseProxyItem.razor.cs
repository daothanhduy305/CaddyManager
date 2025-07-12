using CaddyManager.Contracts.Caddy;
using CaddyManager.Models.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy.CaddyReverseProxies;

/// <summary>
/// Caddy reverse proxy item component that displays the Caddy configuration file name along with other information
/// such as the number of hostnames and ports and allows the user to edit it
/// </summary>
public partial class CaddyReverseProxyItem : ComponentBase
{
    /// <summary>
    /// File path of the Caddy configuration file
    /// </summary>
    [Parameter]
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Callback to refresh the Caddy reverse proxies on the main page
    /// </summary>
    [Parameter]
    public EventCallback OnCaddyRestartRequired { get; set; }
    
    /// <summary>
    /// Dialog service for showing the Caddy file editor dialog
    /// </summary>
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
    /// <summary>
    /// Caddy service for getting the Caddy configuration file information
    /// </summary>
    [Inject]
    private ICaddyService CaddyService { get; set; } = null!;
    
    private CaddyConfigurationInfo ConfigurationInfo { get; set; } = new();
    
    /// <summary>
    /// Refresh the current state of the component.
    /// </summary>
    private void Refresh()
    {
        ConfigurationInfo = CaddyService.GetCaddyConfigurationInfo(FileName);
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender) return;
        
        Refresh();
    }

    /// <summary>
    /// Show the Caddy file editor dialog
    /// </summary>
    /// <returns></returns>
    private async Task Edit()
    {
        var dialog = await DialogService.ShowAsync<CaddyfileEditor.CaddyfileEditor>("Caddy file", options: new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.Medium,
        }, parameters: new DialogParameters
        {
            { "FileName", FileName }
        });

        var result = await dialog.Result;
        
        if (result is { Data: bool, Canceled: false } && (bool)result.Data)
        {
            await OnCaddyRestartRequired.InvokeAsync();
        }

        Refresh();
    }
}