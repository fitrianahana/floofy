using floofy.ViewModels;

namespace floofy.Views;

public partial class EditProfile : ContentPage
{
  private readonly EditProfileViewModel _viewModel;

  public EditProfile(EditProfileViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;

    _viewModel.OnSaved = async () =>
    {
      await Task.Delay(700);
      await Shell.Current.GoToAsync("..");
    };
  }

  protected override void OnAppearing()
  {
    base.OnAppearing();
    _viewModel.LoadFromSession();
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }
}
