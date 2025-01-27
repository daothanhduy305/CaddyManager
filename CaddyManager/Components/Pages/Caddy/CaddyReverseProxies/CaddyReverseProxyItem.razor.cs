using CaddyManager.Contracts.Caddy;
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

    private Task Edit()
    {
        return DialogService.ShowAsync<CaddyfileEditor.CaddyfileEditor>("Caddy file", options: new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.Medium,
        }, parameters: new DialogParameters
        {
            { "FileName", FileName }
        });
    }
}