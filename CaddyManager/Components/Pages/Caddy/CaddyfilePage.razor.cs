using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy;

/// <summary>
/// Caddyfile page component that allows the user to edit the global Caddy configuration file
/// </summary>
public partial class CaddyfilePage : ComponentBase
{
    /// <summary>
    /// Content of the Caddy configuration file
    /// </summary>
    private string _caddyConfigurationContent = string.Empty;

    /// <summary>
    /// Code editor for the Caddy configuration file
    /// </summary>
    private StandaloneCodeEditor _codeEditor = null!;

    /// <summary>
    /// Caddy service for getting the Caddy configuration file information
    /// </summary>
    [Inject] private ICaddyService CaddyService { get; set; } = null!;

    /// <summary>
    /// Snackbar service for displaying messages to the user
    /// </summary>
    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    /// <summary>
    /// Initializes the component
    /// </summary>
    protected override Task OnInitializedAsync()
    {
        // Load the content of the Caddy configuration file
        _caddyConfigurationContent = CaddyService.GetCaddyGlobalConfigurationContent();
        return base.OnInitializedAsync();
    }

    /// <summary>
    /// Returns the construction options for the Caddy configuration file editor
    /// </summary>
    /// <param name="editor">The Caddy configuration file editor</param>
    /// <returns>The construction options for the Caddy configuration file editor</returns>
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

    /// <summary>
    /// Saves the Caddy configuration file
    /// </summary>
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

    /// <summary>
    /// Cancels the Caddy configuration file editor
    /// </summary>
    private void Cancel()
    {
        _codeEditor.SetValue(_caddyConfigurationContent);
    }
}