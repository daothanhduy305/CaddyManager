using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.CaddyfileEditor;

public partial class CaddyfileEditor : ComponentBase
{
    private string _caddyConfigurationContent = string.Empty;
    
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;
    
    /// <summary>
    /// Determines if the Caddy configuration file is new
    /// </summary>
    private bool IsNew => string.IsNullOrWhiteSpace(FileName);
    
    [Parameter]
    public string FileName { get; set; } = string.Empty;

    protected override Task OnInitializedAsync()
    {
        // Load the content of the Caddy configuration file
        _caddyConfigurationContent = CaddyService.GetCaddyConfigurationContent(FileName);
        return base.OnInitializedAsync();
    }
    
    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = "graphql",
            Value = _caddyConfigurationContent,
            Theme = "vs-dark",
        };
    }

    private void Submit() => MudDialog.Close(DialogResult.Ok(true));

    private void Cancel() => MudDialog.Cancel();
}