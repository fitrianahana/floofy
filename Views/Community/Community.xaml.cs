using System.Windows.Input;
using floofy.ViewModels;

namespace floofy.Views;

public partial class Community : ContentPage
{
  private readonly CommunityViewModel _viewModel;
  public Community(CommunityViewModel viewModel)
  {
    InitializeComponent();
    _viewModel = viewModel;
    BindingContext = viewModel;
  }
  protected override void OnAppearing()
  {
    base.OnAppearing();
    if (_viewModel.LoadPostsCommand is ICommand postsCmd && postsCmd.CanExecute(null))
      postsCmd.Execute(null);
    if (_viewModel.LoadEventsCommand is ICommand eventsCmd && eventsCmd.CanExecute(null))
      eventsCmd.Execute(null);
  }
}