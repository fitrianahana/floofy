using floofy.ViewModels;
using System.Windows.Input;
namespace floofy.Views;

public partial class PetShop : ContentPage
{
  private readonly PetListViewModel _viewModel;
  public PetShop(PetListViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }
  protected override async void OnAppearing()
  {
    base.OnAppearing();
    // Load pets when page appears
    if (_viewModel.LoadPetsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
  private async void OnSearchButtonPressed(object sender, EventArgs e)
  {
    // Execute search command when search button is pressed
    if (_viewModel.SearchPetsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
}