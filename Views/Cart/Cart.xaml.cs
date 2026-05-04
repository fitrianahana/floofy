using System.Windows.Input;
using floofy.ViewModels;

namespace floofy.Views;

public partial class Cart : ContentPage
{
  private readonly CartViewModel _viewModel;

  public Cart(CartViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    await _viewModel.LoadAsync();
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }

  private void OnClearClicked(object? sender, EventArgs e)
  {
    if (_viewModel.ClearCartCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
}
