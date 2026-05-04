namespace floofy.Views;

public partial class CancelListingDialog : ContentPage
{
  public bool ConfirmedCancel { get; private set; } = false;
  private bool _isClosing = false;
  private readonly TaskCompletionSource<bool> _closingTcs = new();

  public CancelListingDialog(string petName)
  {
    InitializeComponent();
    if (!string.IsNullOrWhiteSpace(petName))
    {
      MessageLabel.Text = $"Are you sure you want to cancel rehoming for {petName}? The listing will be removed from the shop and adopters won't be able to find it.";
    }
  }

  public Task<bool> WaitForResultAsync() => _closingTcs.Task;

  private void OnKeepClicked(object? sender, EventArgs e)
  {
    if (_isClosing) return;
    _isClosing = true;
    ConfirmedCancel = false;
    _closingTcs.TrySetResult(false);

    MainThread.BeginInvokeOnMainThread(async () =>
    {
      try { await Navigation!.PopModalAsync(); }
      catch { }
    });
  }

  private void OnConfirmCancelClicked(object? sender, EventArgs e)
  {
    if (_isClosing) return;
    _isClosing = true;
    ConfirmedCancel = true;
    _closingTcs.TrySetResult(true);

    MainThread.BeginInvokeOnMainThread(async () =>
    {
      try { await Navigation!.PopModalAsync(); }
      catch { }
    });
  }
}
