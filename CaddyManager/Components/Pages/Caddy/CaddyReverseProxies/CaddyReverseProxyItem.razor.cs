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
    /// Callback to refresh the Caddy reverse proxies on the main page
    /// </summary>
    [Parameter]
    public EventCallback OnCaddyRestartRequired { get; set; }
    
    [Parameter]
    public EventCallback<string> OnCaddyfileDuplicateRequested { get; set; }

    [Parameter]
    public CaddyConfigurationInfo ConfigurationInfo { get; set; } = null!;
    
    /// <summary>
    /// Dialog service for showing the Caddy file editor dialog
    /// </summary>
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

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
        }, parameters: new DialogParameters<CaddyfileEditor.CaddyfileEditor>
        {
            { p => p.FileName, ConfigurationInfo.FileName },
            { p => p.OnDuplicate, EventCallback.Factory.Create(this, OnCaddyfileDuplicateRequested) }
        });

        var result = await dialog.Result;
        
        if (result is { Data: bool, Canceled: false } && (bool)result.Data)
        {
            await OnCaddyRestartRequired.InvokeAsync();
        }
    }
}