using System.Globalization;
using CaddyManager.Components.Pages.Generic;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Docker;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Caddy.CaddyReverseProxies;

/// <summary>
/// Page to manage reverse proxy configurations in the form of *.caddy files
/// </summary>
public partial class CaddyReverseProxiesPage : ComponentBase
{
    private bool _isProcessing;
    private List<string> _availableCaddyConfigurations = [];
    private IReadOnlyCollection<string> _selectedCaddyConfigurations = [];

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
        var dialog = await DialogService.ShowAsync<CaddyfileEditor.CaddyfileEditor>("New configuration",
            options: new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
            }, parameters: new DialogParameters
            {
                { "FileName", string.Empty }
            });

        var result = await dialog.Result;

        if (result is { Data: bool, Canceled: false } && (bool)result.Data)
        {
            Refresh();
        }
    }

    /// <summary>
    /// Get the latest information from the server
    /// </summary>
    private void Refresh()
    {
        _availableCaddyConfigurations = CaddyService.GetExistingCaddyConfigurations();
        StateHasChanged();
    }

    /// <summary>
    /// Have the selected configurations be deleted
    /// </summary>
    private Task Delete()
    {
        var confWord = _selectedCaddyConfigurations.Count > 1 ? "configurations" : "configuration";

        return DialogService.ShowAsync<ConfirmationDialog>($"Delete {confWord}", options: new DialogOptions
        {
            FullWidth = true,
            MaxWidth = MaxWidth.ExtraSmall,
        }, parameters: new DialogParameters
        {
            {
                "Message",
                $"Are you sure you want to delete the selected {confWord}?"
            },
            {
                "OnConfirm", EventCallback.Factory.Create(this, () =>
                {
                    var response = CaddyService.DeleteCaddyConfigurations(_selectedCaddyConfigurations.ToList());

                    _selectedCaddyConfigurations =
                        _selectedCaddyConfigurations.Except(response.DeletedConfigurations).ToList();

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
            { "ConfirmText", "Yes" },
            { "ConfirmColor", Color.Error },
            { "CancelText", "No" }
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
}