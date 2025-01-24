using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages;

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
            Language = "plaintext",
            Value = _caddyConfigurationContent,
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
        // CaddyService.GetCaddyGlobalConfigurationContent();
    }
}