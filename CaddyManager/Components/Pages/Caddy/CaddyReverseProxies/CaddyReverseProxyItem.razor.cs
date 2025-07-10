using CaddyManager.Contracts.Caddy;
using CaddyManager.Models.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy.CaddyReverseProxies;

public partial class CaddyReverseProxyItem : ComponentBase
{
    /// <summary>
    /// File path of the Caddy configuration file
    /// </summary>
    [Parameter]
    public string FileName { get; set; } = string.Empty;
    
    [Inject]
    private IDialogService DialogService { get; set; } = null!;
    
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

        await dialog.Result;
        
        Refresh();
    }
}