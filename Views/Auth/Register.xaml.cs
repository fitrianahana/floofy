using floofy.ViewModels;
namespace floofy.Views;

public partial class Register : ContentPage
{
  private readonly RegisterViewModel _viewModel;
  public Register(RegisterViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;

    viewModel.PropertyChanged += async (s, e) =>
    {
      if (e.PropertyName == nameof(RegisterViewModel.IsLoading) && !viewModel.IsLoading)
      {
        if (viewModel.IsRegistrationSuccessful)
        {
          await Navigation.PopAsync();
        }
      }
    };
  }
}