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
}