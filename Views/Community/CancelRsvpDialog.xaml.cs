namespace floofy.Views;

public partial class CancelRsvpDialog : ContentPage
{
  public bool ConfirmedCancel { get; private set; } = false;
  private bool _isClosing = false;
  private TaskCompletionSource<bool>? _closingTcs;

  public CancelRsvpDialog()
  {
    InitializeComponent();
    System.Diagnostics.Debug.WriteLine("[DIALOG] CancelRsvpDialog created");
    _closingTcs = new TaskCompletionSource<bool>();
  }

  /// <summary>
  /// Waits for the dialog to close and returns whether the user confirmed the cancel.
  /// </summary>
  public Task<bool> WaitForResultAsync()
  {
    return _closingTcs?.Task ?? Task.FromResult(false);
  }

  private void OnKeepClicked(object? sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("[DIALOG] Keep RSVP button clicked");
    if (_isClosing) return;
    _isClosing = true;
    
    ConfirmedCancel = false;
    System.Diagnostics.Debug.WriteLine("[DIALOG] ConfirmedCancel set to false");
    
    // Signal the waiting task
    _closingTcs?.TrySetResult(false);
    
    // Close the dialog on the main thread
    MainThread.BeginInvokeOnMainThread(async () =>
    {
      try
      {
        System.Diagnostics.Debug.WriteLine("[DIALOG] Keep: About to PopModalAsync");
        await Navigation!.PopModalAsync();
        System.Diagnostics.Debug.WriteLine("[DIALOG] Keep: PopModalAsync completed");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[DIALOG] Keep: ERROR in PopModalAsync: {ex.Message}");
      }
    });
  }

  private void OnConfirmCancelClicked(object? sender, EventArgs e)
  {
    System.Diagnostics.Debug.WriteLine("[DIALOG] *** YES, CANCEL BUTTON CLICKED ***");
    if (_isClosing) return;
    _isClosing = true;
    
    ConfirmedCancel = true;
    System.Diagnostics.Debug.WriteLine("[DIALOG] ConfirmedCancel set to TRUE");
    
    // Signal the waiting task
    _closingTcs?.TrySetResult(true);
    
    // Close the dialog on the main thread
    MainThread.BeginInvokeOnMainThread(async () =>
    {
      try
      {
        System.Diagnostics.Debug.WriteLine("[DIALOG] Cancel: About to PopModalAsync");
        await Navigation!.PopModalAsync();
        System.Diagnostics.Debug.WriteLine("[DIALOG] Cancel: PopModalAsync completed");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[DIALOG] Cancel: ERROR in PopModalAsync: {ex.Message}");
      }
    });
  }
}
