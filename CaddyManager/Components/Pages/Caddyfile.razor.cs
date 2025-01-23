using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;

namespace CaddyManager.Components.Pages;

public partial class Caddyfile: ComponentBase
{
    private string _caddyConfigurationContent = string.Empty;

    protected override Task OnInitializedAsync()
    {
        // Load the content of the Caddy configuration file
        _caddyConfigurationContent = CaddyService.GetCaddyGlobalConfigurationContent();
        return base.OnInitializedAsync();
    }
    
    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = "plaintext",
            Value = _caddyConfigurationContent,
        };
    }
    
    private void Submit()
    {
        // CaddyService.SaveCaddyGlobalConfigurationContent(_caddyConfigurationContent);
    }
    
    private void Cancel()
    {
        // CaddyService.GetCaddyGlobalConfigurationContent();
    }
}