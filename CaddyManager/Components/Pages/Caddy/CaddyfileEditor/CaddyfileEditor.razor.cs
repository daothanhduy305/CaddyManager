using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Models.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using CaddyManager.Contracts.Docker;

namespace CaddyManager.Components.Pages.Caddy.CaddyfileEditor;

/// <summary>
/// Caddyfile editor component that allows the user to edit the Caddy configuration file
/// </summary>
public partial class CaddyfileEditor : ComponentBase
{
    private string _caddyConfigurationContent = string.Empty;
    private StandaloneCodeEditor _codeEditor = null!;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    /// <summary>
    /// Determines if the Caddy configuration file is new
    /// </summary>
    private bool IsNew { get; set; }

    [Parameter] public string FileName { get; set; } = string.Empty;

    [Inject] private ICaddyService CaddyService { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IDockerService DockerService { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        IsNew = string.IsNullOrWhiteSpace(FileName);

        if (!IsNew)
        {
            // Load the content of the Caddy configuration file
            _caddyConfigurationContent = CaddyService.GetCaddyConfigurationContent(FileName);
        }

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
        var response = CaddyService.SaveCaddyConfiguration(new CaddySaveConfigurationRequest
        {
            IsNew = IsNew,
            FileName = FileName,
            Content = await _codeEditor.GetValue(),
        });

        if (response.Success)
        {
            Snackbar.Add($"{FileName} Caddy configuration saved successfully", Severity.Success);
            MudDialog.Close(DialogResult.Ok(false)); // Indicate successful save but no restart
        }
        else
        {
            Snackbar.Add(response.Message, Severity.Error);
            MudDialog.Close(DialogResult.Ok(false)); // Indicate failed save
        }
    }

    /// <summary>
    /// Cancels the Caddy configuration file editor
    /// </summary>
    private void Cancel()
    {
        MudDialog.Cancel();
    }

    /// <summary>
    /// Saves the Caddy configuration file and restarts the Caddy container
    /// </summary>
    private async Task SaveAndRestart()
    {
        var submitResponse = CaddyService.SaveCaddyConfiguration(new CaddySaveConfigurationRequest
        {
            IsNew = IsNew,
            FileName = FileName,
            Content = await _codeEditor.GetValue(),
        });

        if (submitResponse.Success)
        {
            Snackbar.Add($"{FileName} Caddy configuration saved successfully", Severity.Success);
            // Indicate successful save and that a restart is required by the calling component
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Snackbar.Add(submitResponse.Message, Severity.Error);
            // Indicate failed save, no restart needed
            MudDialog.Close(DialogResult.Ok(false));
        }
    }
}