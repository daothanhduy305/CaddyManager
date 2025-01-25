using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy;

public partial class CaddyfilePage : ComponentBase
{
    private string _caddyConfigurationContent = string.Empty;
    private StandaloneCodeEditor _codeEditor = null!;

    [Inject] private ICaddyService CaddyService { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

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
            Language = "graphql",
            Value = _caddyConfigurationContent,
            Theme = "vs-dark",
            Padding = new EditorPaddingOptions
            {
                Top = 8f,
                Bottom = 8f,
            }
        };
    }

    private async Task Submit()
    {
        var response = CaddyService.SaveCaddyGlobalConfiguration(await _codeEditor.GetValue());

        if (response.Success)
        {
            Snackbar.Add("Caddy configuration saved successfully", Severity.Success);
        }
        else
        {
            Snackbar.Add("Failed to save Caddy configuration", Severity.Error);
        }
    }

    private void Cancel()
    {
        _codeEditor.SetValue(_caddyConfigurationContent);
    }
}