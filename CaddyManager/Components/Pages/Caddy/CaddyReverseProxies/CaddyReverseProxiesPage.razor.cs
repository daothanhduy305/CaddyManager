using System.Globalization;
using CaddyManager.Components.Pages.Generic;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Docker;
using CaddyManager.Models.Caddy;
using Humanizer;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using CaddyfileEditorComponent = CaddyManager.Components.Pages.Caddy.CaddyfileEditor.CaddyfileEditor;

namespace CaddyManager.Components.Pages.Caddy.CaddyReverseProxies;

/// <summary>
/// Page to manage reverse proxy configurations in the form of *.caddy files
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global
public partial class CaddyReverseProxiesPage : ComponentBase
{
    private bool _isProcessing;
    private List<CaddyConfigurationInfo> _availableCaddyConfigurations = [];
    private IReadOnlyCollection<CaddyConfigurationInfo> _selectedCaddyConfigurations = [];
    private string _debouncedText = string.Empty;

    [Inject] private ICaddyService CaddyService { get; set; } = null!;

    [Inject] private IDockerService DockerService { get; set; } = null!;

    [Inject] private IDialogService DialogService { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Refresh();
        }
    }

    /// <summary>
    /// Method to help open the dialog to create a new reverse proxy configuration
    /// </summary>
    /// <returns></returns>
    private async Task NewReverseProxy()
    {
        var dialog = await DialogService.ShowAsync<CaddyfileEditorComponent>("New configuration",
            options: new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium
            }, parameters: new DialogParameters<CaddyfileEditorComponent>
            {
                { p => p.FileName, string.Empty }
            });

        var result = await dialog.Result;

        if (result is { Data: bool, Canceled: false } && (bool)result.Data)
        {
            Refresh();
            await RestartCaddy();
        }
    }

    /// <summary>
    /// Get the latest information from the server
    /// </summary>
    private void Refresh()
    {
        var notSearching = string.IsNullOrWhiteSpace(_debouncedText);
        _availableCaddyConfigurations = [..CaddyService.GetExistingCaddyConfigurations()
            .Where(conf => notSearching || conf.FileName.Contains(_debouncedText, StringComparison.OrdinalIgnoreCase) || conf.ReverseProxyHostname.Contains(_debouncedText, StringComparison.OrdinalIgnoreCase))
            .OrderBy(conf => conf.FileName)];
        StateHasChanged();
    }

    /// <summary>
    /// Have the selected configurations be deleted
    /// </summary>
    private Task Delete()
    {
        var confWord = "configuration".ToQuantity(_selectedCaddyConfigurations.Count, ShowQuantityAs.None);

        return DialogService.ShowAsync<ConfirmationDialog>($"Delete {_selectedCaddyConfigurations.Count} {confWord}", options: new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraSmall
        }, parameters: new DialogParameters<ConfirmationDialog>
        {
            {
                p => p.Message,
                $"Are you sure to delete the selected {confWord}?\n\n" +
                $"{string.Join("\n", _selectedCaddyConfigurations.Select(c => $"⏵\t{c}"))}"
            },
            {
                p => p.OnConfirm, EventCallback.Factory.Create(this, () =>
                {
                    var response = CaddyService.DeleteCaddyConfigurations(_selectedCaddyConfigurations.Select(c => c.FileName).ToList());

                    _selectedCaddyConfigurations =
                        _selectedCaddyConfigurations.Where(c => !response.DeletedConfigurations.Contains(c.FileName)).ToList();

                    if (response.Success)
                    {
                        Snackbar.Add(
                            $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(confWord)} deleted successfully",
                            Severity.Success);
                        Refresh();
                    }
                    else
                    {
                        Snackbar.Add(response.Message, Severity.Error);
                    }
                })
            },
            { p => p.ConfirmText, "Yes" },
            { p => p.ConfirmColor, Color.Error },
            { p => p.CancelText, "No" }
        });
    }

    /// <summary>
    /// Restart the Caddy container
    /// </summary>
    /// <returns></returns>
    private async Task RestartCaddy()
    {
        try
        {
            _isProcessing = true;
            StateHasChanged();
            Snackbar.Add("Restarting Caddy container", Severity.Info);
            // Added a small delay for debugging purposes to ensure UI renders
            await Task.Delay(100);
            await DockerService.RestartCaddyContainerAsync();
            Snackbar.Add("Caddy container restarted successfully", Severity.Success);
            _isProcessing = false;
            StateHasChanged();
        }
        catch
        {
            Snackbar.Add("Failed to restart the Caddy container", Severity.Error);
        }
    }
    
    /// <summary>
    /// Handle the interval elapsed event for debounced text input for search functionality.
    /// </summary>
    /// <param name="debouncedText"></param>
    private void HandleIntervalElapsed(string debouncedText)
    {
        // Simply refresh the page with the new debounced text
        Refresh();
    }
    
    /// <summary>
    /// Handle the click event for the search bar adornment. If the debounced text is empty, then simply refresh
    /// to have the search be effective, otherwise, clear the debounced text to reset the search.
    /// </summary>
    private void HandleSearchBarAdornmentClick()
    {
        if (!string.IsNullOrWhiteSpace(_debouncedText))
        {
            _debouncedText = string.Empty;
        }
        
        Refresh();
    }
}