using System.Windows.Input;
using floofy.ViewModels;

namespace floofy.Views;

public partial class Community : ContentPage
{
  private readonly CommunityViewModel _viewModel;
  private bool _isPostsTabActive = true;

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

  private void OnPostsTabClicked(object? sender, EventArgs e)
  {
    _isPostsTabActive = true;
    UpdateTabUI();
    PostsTab.IsVisible = true;
    EventsTab.IsVisible = false;
  }

  private void OnEventsTabClicked(object? sender, EventArgs e)
  {
    _isPostsTabActive = false;
    UpdateTabUI();
    PostsTab.IsVisible = false;
    EventsTab.IsVisible = true;
    AvailableEventsSection.IsVisible = true;
    MyRsvpsSection.IsVisible = false;
  }

  private void OnAvailableEventsClicked(object? sender, EventArgs e)
  {
    AvailableEventsSection.IsVisible = true;
    MyRsvpsSection.IsVisible = false;
    AvailableEventsButton.BackgroundColor = Color.FromArgb("#B19CD9");
    AvailableEventsButton.TextColor = Colors.White;
    MyRsvpButton.BackgroundColor = Colors.Transparent;
    MyRsvpButton.TextColor = Color.FromArgb("#6B5B8C");
  }

  private void OnMyRsvpClicked(object? sender, EventArgs e)
  {
    AvailableEventsSection.IsVisible = false;
    MyRsvpsSection.IsVisible = true;
    AvailableEventsButton.BackgroundColor = Colors.Transparent;
    AvailableEventsButton.TextColor = Color.FromArgb("#6B5B8C");
    MyRsvpButton.BackgroundColor = Color.FromArgb("#B19CD9");
    MyRsvpButton.TextColor = Colors.White;
  }

  private void UpdateTabUI()
  {
    if (_isPostsTabActive)
    {
      PostsTabButton.TextColor = Color.FromArgb("#2D1B4E");
      PostsTabButton.FontAttributes = FontAttributes.Bold;
      EventsTabButton.TextColor = Color.FromArgb("#6B5B8C");
      EventsTabButton.FontAttributes = FontAttributes.None;
      // Reset translation to position under Posts tab
      TabIndicator.TranslationX = 0;
    }
    else
    {
      PostsTabButton.TextColor = Color.FromArgb("#6B5B8C");
      PostsTabButton.FontAttributes = FontAttributes.None;
      EventsTabButton.TextColor = Color.FromArgb("#2D1B4E");
      EventsTabButton.FontAttributes = FontAttributes.Bold;
      // Animate indicator to Events tab position (Posts width ~60 + spacing 24 = ~84px)
      MainThread.BeginInvokeOnMainThread(async () =>
      {
        await TabIndicator.TranslateToAsync(82, 0, 200, Easing.Linear);
      });
    }
  }
}