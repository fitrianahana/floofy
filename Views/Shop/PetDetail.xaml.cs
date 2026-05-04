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
         // Navigate back to shop page
         await Shell.Current.GoToAsync("..");
       }
       else
       {
         // Show error if cancellation failed
         await DisplayAlertAsync("Error", _viewModel.ErrorMessage, "OK");
       }
     }
  }
}