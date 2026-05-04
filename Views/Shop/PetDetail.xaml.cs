using floofy.ViewModels;

namespace floofy.Views;

[QueryProperty(nameof(PetId), "petId")]
public partial class PetDetail : ContentPage
{
  private readonly PetDetailViewModel _viewModel;
  private Guid _petId = Guid.Empty;

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
        // Show success toast on current page before navigating
        await ShowSuccessToastAsync($"{petName}'s listing was cancelled");
        
        // Then navigate back after toast completes (3 seconds)
        await Shell.Current.GoToAsync("..");
      }
      else
      {
        // Show error if cancellation failed
        await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "OK");
      }
    }
  }

  private async Task ShowSuccessToastAsync(string message)
  {
    // Set the toast message
    SuccessToastMessage.Text = message;
    SuccessToast.Opacity = 0;
    SuccessToast.IsVisible = true;
    
    // Animate in
    await SuccessToast.FadeToAsync(1, 200, Easing.CubicOut);
    
    // Show for 3 seconds
    await Task.Delay(3000);
    
    // Animate out
    await SuccessToast.FadeToAsync(0, 250, Easing.CubicIn);
    SuccessToast.IsVisible = false;
  }
}