using floofy.ViewModels;

namespace floofy.Views;

public partial class SellPet : ContentPage
{
  private readonly SellPetViewModel _viewModel;

  public SellPet(SellPetViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;

    _viewModel.OnSubmitted = async () =>
    {
      await Task.Delay(900);
      _viewModel.Reset();
      await Shell.Current.GoToAsync("..");
    };
  }

  protected override void OnAppearing()
  {
    base.OnAppearing();
    _viewModel.Reset();
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }

  private async void OnViewAgreementClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("rehomingAgreement");
  }

  private async void OnViewPolicyClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("rehomingPolicy");
  }
}
