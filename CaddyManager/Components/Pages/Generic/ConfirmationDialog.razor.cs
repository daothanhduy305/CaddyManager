using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CaddyManager.Components.Pages.Generic;

public partial class ConfirmationDialog : ComponentBase
{
    [CascadingParameter] 
    private IMudDialogInstance MudDialog { get; set; } = null!;
    
    /// <summary>
    /// The message to display to the user, to be set in the body of the dialog
    /// </summary>
    [Parameter]
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// Callback when the user confirms the action. The dialog will close after this callback is invoked.
    /// </summary>
    [Parameter]
    public EventCallback OnConfirm { get; set; } = EventCallback.Empty;
    
    /// <summary>
    /// Callback when the user cancels the action. The dialog will close after this callback is invoked.
    /// </summary>
    [Parameter]
    public EventCallback OnCancel { get; set; } = EventCallback.Empty;
    
    /// <summary>
    /// The text to display on the confirm button
    /// </summary>
    [Parameter]
    public string ConfirmText { get; set; } = "Confirm";
    
    /// <summary>
    /// The color of the confirm button
    /// </summary>
    [Parameter]
    public Color ConfirmColor { get; set; } = Color.Primary;
    
    /// <summary>
    /// The text to display on the cancel button
    /// </summary>
    [Parameter]
    public string CancelText { get; set; } = "Cancel";

    /// <summary>
    /// Perform the confirmation action
    /// </summary>
    private async Task Confirm()
    {
        await OnConfirm.InvokeAsync();
        MudDialog.Close();
    }

    /// <summary>
    /// Cancel the cancel action
    /// </summary>
    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
        MudDialog.Close();
    }
}