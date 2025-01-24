using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.ReverseProxies;

public partial class ReverseProxyItem : ComponentBase
{
    /// <summary>
    /// File path of the Caddy configuration file
    /// </summary>
    [Parameter]
    public string FileName { get; set; } = string.Empty;

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