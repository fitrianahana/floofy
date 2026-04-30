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
    CreatePostSection.IsVisible = true;
  }

  private void OnEventsTabClicked(object? sender, EventArgs e)
  {
    _isPostsTabActive = false;
    UpdateTabUI();
    PostsTab.IsVisible = false;
    EventsTab.IsVisible = true;
    CreatePostSection.IsVisible = false;
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

   private async void OnCancelRsvpButtonClicked(object? sender, EventArgs e)
   {
     System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] *** CANCEL RSVP BUTTON CLICKED ***");
     
     try
     {
       System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Sender type: " + sender?.GetType().Name);
       if (sender is not Button button)
       {
         System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] ERROR: sender is not a Button");
         return;
       }

       System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Got button, checking binding context");
       System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] BindingContext type: " + button.BindingContext?.GetType().Name);
       
       // Get the binding context which should be CommunityEventItem
       if (button.BindingContext is not floofy.ViewModels.CommunityEventItem eventItem)
       {
         System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ERROR: BindingContext is {button.BindingContext?.GetType().Name ?? "null"}");
         return;
       }

       var eventId = eventItem.EventId;
       System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] Got eventId from binding context: {eventId}");
       System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] Event name: {eventItem.Name}");

       // Show the dialog with callback
       System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] About to show dialog");
       await ShowCancelRsvpDialogAsync(eventId);
       System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Dialog shown");
     }
     catch (Exception ex)
     {
       System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ERROR in OnCancelRsvpButtonClicked: {ex}");
       System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ERROR Message: {ex.Message}");
     }
   }

    private async Task ShowCancelRsvpDialogAsync(Guid eventId)
    {
      System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ======================== ShowCancelRsvpDialogAsync STARTED with eventId: {eventId} ========================");

      CancelRsvpDialog? dialog = null;
      try
      {
        System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Creating dialog");
        
        // Create the dialog
        dialog = new CancelRsvpDialog();

        System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Dialog created, about to push modal");
        
        // Show the dialog
        System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] About to PushModalAsync");
        await Navigation.PushModalAsync(dialog);
        System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] PushModalAsync completed");
        
        // Wait for the dialog to close and get the result
        System.Diagnostics.Debug.WriteLine("[CODE-BEHIND] Waiting for dialog result...");
        var userConfirmedCancel = await dialog.WaitForResultAsync();
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] Dialog result received: ConfirmedCancel = {userConfirmedCancel}");
        
        // Check the result
        if (userConfirmedCancel)
        {
          System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ✓ User CONFIRMED cancel for event: {eventId}");
          System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] About to call CancelRsvpDirectAsync");
          await _viewModel.CancelRsvpDirectAsync(eventId);
          System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] CancelRsvpDirectAsync completed successfully");
        }
        else
        {
          System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ✗ User kept their RSVP (ConfirmedCancel=false)");
        }
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] *** EXCEPTION in ShowCancelRsvpDialogAsync ***");
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] Exception Type: {ex.GetType().Name}");
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] Exception Message: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] StackTrace: {ex.StackTrace}");
      }
      finally
      {
        System.Diagnostics.Debug.WriteLine($"[CODE-BEHIND] ======================== ShowCancelRsvpDialogAsync FINISHED ========================");
      }
    }
}