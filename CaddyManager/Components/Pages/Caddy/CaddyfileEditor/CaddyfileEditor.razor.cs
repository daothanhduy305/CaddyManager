using BlazorMonaco.Editor;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Models.Caddy;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy.CaddyfileEditor;

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
            MudDialog.Close(DialogResult.Ok(true));
        }
        else
        {
            Snackbar.Add("Failed to save Caddy configuration", Severity.Error);
        }
    }

    private void Cancel() => MudDialog.Cancel();
}