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
  protected override void OnAppearing()
  {
    base.OnAppearing();
    if (_viewModel.LoadPetsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }

  private void OnSearchButtonPressed(object sender, EventArgs e)
  {
    if (_viewModel.SearchPetsCommand is ICommand cmd && cmd.CanExecute(null))
    {
      cmd.Execute(null);
    }
  }
}