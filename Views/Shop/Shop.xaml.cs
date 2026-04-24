using floofy.ViewModels;
using System.Windows.Input;
namespace floofy.Views;

public partial class Shop : ContentPage
{
    private readonly ProductListViewModel _viewModel;
    public Shop(ProductListViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Load products when page appears
        if (_viewModel.LoadProductsCommand is ICommand cmd && cmd.CanExecute(null))
        {
            cmd.Execute(null);
        }
    }
    private async void OnSearchButtonPressed(object sender, EventArgs e)
    {
        // Execute search command when search button is pressed
        if (_viewModel.SearchCommand is ICommand cmd && cmd.CanExecute(null))
        {
            cmd.Execute(null);
        }
    }
}