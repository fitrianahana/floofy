using floofy.ViewModels;
using System.Windows.Input;

namespace floofy.Views;

public partial class Shop : ContentPage
{
  private readonly ShopViewModel _viewModel;
  public Shop(ShopViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    if (_viewModel.LoadCurrentSectionCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
  private async void OnSearchButtonPressed(object sender, EventArgs e)
  {
    if (_viewModel.SearchCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
}