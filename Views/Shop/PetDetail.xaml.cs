using floofy.ViewModels;

namespace floofy.Views;

[QueryProperty(nameof(PetId), "petId")]
public partial class PetDetail : ContentPage
{
  private readonly PetDetailViewModel _viewModel;
  private Guid _petId = Guid.Empty;
  
  public static event EventHandler<string>? OnListingCancelled;
  public static string? PendingCancelNotification { get; set; }

  public string PetId
  {
    set
    {
      if (Guid.TryParse(Uri.UnescapeDataString(value ?? string.Empty), out var parsed))
      {
        _petId = parsed;
      }
    }
  }

  public PetDetail(PetDetailViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    if (_petId != Guid.Empty)
    {
      await _viewModel.LoadPetAsync(_petId);
    }
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }

  private async void OnCancelListingClicked(object? sender, EventArgs e)
  {
    if (_viewModel.Pet is null) return;

    var dialog = new CancelListingDialog(_viewModel.Pet.Name);
    await Navigation.PushModalAsync(dialog);
    var confirmed = await dialog.WaitForResultAsync();

      if (confirmed)
      {
        var petName = _viewModel.Pet.Name;
        await _viewModel.OnCancelListingAsync();
        
        if (string.IsNullOrEmpty(_viewModel.ErrorMessage))
        {
          var message = $"{petName}'s listing was cancelled";
          // Store in both event and static property for reliability
          PendingCancelNotification = message;
          System.Diagnostics.Debug.WriteLine($"[PetDetail] Set PendingCancelNotification: {message}");
          
          // Navigate back
          await Shell.Current.GoToAsync("..");
          
          // Give time for navigation to complete, then show toast
          await Task.Delay(500);
          System.Diagnostics.Debug.WriteLine($"[PetDetail] After navigation, raising event");
          OnListingCancelled?.Invoke(this, message);
          System.Diagnostics.Debug.WriteLine($"[PetDetail] Raised OnListingCancelled event");
        }
        else
        {
          // Show error if cancellation failed
          await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "OK");
        }
      }
  }
}