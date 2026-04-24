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
    // Handle navigation after successful registration
    viewModel.PropertyChanged += async (s, e) =>
    {
      if (e.PropertyName == nameof(RegisterViewModel.IsLoading) && !viewModel.IsLoading)
      {
        // Registration attempt completed (IsLoading changed from true to false)
        // Check if there's an error message
        if (string.IsNullOrEmpty(viewModel.ErrorMessage))
        {
          // Success! No error message means registration succeeded
          await Navigation.PopAsync();  // Return to Login page
        }
      }
    };
  }
}