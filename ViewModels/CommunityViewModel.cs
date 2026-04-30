using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Models.Enums;
using floofy.Services;
using System.Windows.Input;
namespace floofy.ViewModels;

public class CommunityViewModel : BaseViewModel
{
  private readonly ICommunityService _communityService;
  private readonly SessionService _sessionService;

  // Callback for when RSVP is successful - code-behind can subscribe to this
  public Action? OnRsvpSuccessful { get; set; }

  private ObservableCollection<Post> _posts = new();
  private ObservableCollection<CommunityEventItem> _events = new();
  private ObservableCollection<CommunityEventItem> _myRsvpEvents = new();

  private Post? _selectedPost;
  private CommunityEventItem? _selectedEvent;
  private string _newPostTitle = string.Empty;
  private string _newPostContent = string.Empty;
  private PostVisibility _postVisibility = PostVisibility.Public;

  public ObservableCollection<Post> Posts
  {
    get => _posts;
    set => SetProperty(ref _posts, value);
  }

  public ObservableCollection<CommunityEventItem> Events
  {
    get => _events;
    set => SetProperty(ref _events, value);
  }

  public ObservableCollection<CommunityEventItem> MyRsvpEvents
  {
    get => _myRsvpEvents;
    set => SetProperty(ref _myRsvpEvents, value);
  }

  public Post? SelectedPost
  {
    get => _selectedPost;
    set => SetProperty(ref _selectedPost, value);
  }

  public CommunityEventItem? SelectedEvent
  {
    get => _selectedEvent;
    set => SetProperty(ref _selectedEvent, value);
  }

  public string NewPostTitle
  {
    get => _newPostTitle;
    set => SetProperty(ref _newPostTitle, value);
  }

  public string NewPostContent
  {
    get => _newPostContent;
    set => SetProperty(ref _newPostContent, value);
  }

  public PostVisibility PostVisibility
  {
    get => _postVisibility;
    set => SetProperty(ref _postVisibility, value);
  }

  public ICommand LoadPostsCommand { get; }
  public ICommand LoadEventsCommand { get; }
  public ICommand CreatePostCommand { get; }
  public ICommand RSVPToEventCommand { get; }
  public ICommand RSVPAttendingCommand { get; }
  public ICommand CancelRsvpCommand { get; }

   public CommunityViewModel()
   {
     _communityService = App.Services.GetRequiredService<ICommunityService>();
     _sessionService = App.Services.GetRequiredService<SessionService>();
     LoadPostsCommand = new RelayCommand(async () => await OnLoadPostsAsync());
     LoadEventsCommand = new RelayCommand(async () => await OnLoadEventsAsync());
     CreatePostCommand = new RelayCommand(async () => await OnCreatePostAsync());
     RSVPToEventCommand = new RelayCommand<(Guid, RSVPStatus)>(async (param) => await OnRSVPToEventAsync(param.Item1, param.Item2));
     RSVPAttendingCommand = new RelayCommand<Guid>(async (eventId) => await OnRSVPToEventAsync(eventId, RSVPStatus.Attending));
     CancelRsvpCommand = new RelayCommand<Guid>(async (eventId) => await CancelRsvpDirectAsync(eventId));
   }

  private async Task OnLoadPostsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var posts = await _communityService.GetAllPostsAsync();
      // Sort by latest (most recent first)
      var sortedPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();
      Posts = new ObservableCollection<Post>(sortedPosts);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load posts: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnLoadEventsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      await ReloadEventsAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load events: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnCreatePostAsync()
  {
    if (string.IsNullOrWhiteSpace(NewPostTitle) || string.IsNullOrWhiteSpace(NewPostContent))
    {
      ErrorMessage = "Please enter post title and content";
      return;
    }
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        return;
      }
      await _communityService.CreatePostAsync(userId.Value, NewPostTitle, NewPostContent, PostVisibility);
      NewPostTitle = string.Empty;
      NewPostContent = string.Empty;
      var posts = await _communityService.GetAllPostsAsync();
      // Sort by latest (most recent first)
      var sortedPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();
      Posts = new ObservableCollection<Post>(sortedPosts);
      ErrorMessage = "Post created successfully!";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to create post: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task ReloadEventsAsync()
  {
    System.Diagnostics.Debug.WriteLine("[VIEWMODEL] ========== ReloadEventsAsync STARTED ==========");
    var events = await _communityService.GetAllEventsAsync();
    System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Retrieved {events.Count} events from service");
    
    var currentUserId = _sessionService.CurrentUser?.Id;
    System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Current user ID: {currentUserId}");

    HashSet<Guid> rsvpedEventIds = new();
    if (currentUserId != null)
    {
      var rsvps = await _communityService.GetUserEventRSVPsAsync(currentUserId.Value);
      System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Retrieved {rsvps.Count} RSVPs for user");
      
      foreach (var rsvp in rsvps)
      {
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL]   RSVP: eventId={rsvp.EventId}, status={rsvp.RSVPStatus}, isDeleted={rsvp.IsDeleted}");
      }
      
      rsvpedEventIds = rsvps
          .Where(r => r.RSVPStatus == RSVPStatus.Attending || r.RSVPStatus == RSVPStatus.Pending)
          .Select(r => r.EventId)
          .ToHashSet();
      
      System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Filtered to {rsvpedEventIds.Count} Attending/Pending RSVPs");
    }

    // Separate events into available events and user's RSVP'd events
    var availableEventItems = events
        .Where(e => !rsvpedEventIds.Contains(e.Id))
        .Select(e => new CommunityEventItem
        {
          Event = e,
          IsRsvped = false
        })
        .ToList();

    var myRsvpEventItems = events
        .Where(e => rsvpedEventIds.Contains(e.Id))
        .Select(e => new CommunityEventItem
        {
          Event = e,
          IsRsvped = true
        })
        .ToList();

    System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Available events: {availableEventItems.Count}");
    System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] My RSVP events: {myRsvpEventItems.Count}");

    Events = new ObservableCollection<CommunityEventItem>(availableEventItems);
    MyRsvpEvents = new ObservableCollection<CommunityEventItem>(myRsvpEventItems);
    
    System.Diagnostics.Debug.WriteLine("[VIEWMODEL] ========== ReloadEventsAsync FINISHED ==========");
  }

   private async Task OnRSVPToEventAsync(Guid eventId, RSVPStatus status)
   {
     ErrorMessage = string.Empty;
     IsLoading = true;
     try
     {
       var userId = _sessionService.CurrentUser?.Id;
       if (userId == null)
       {
         ErrorMessage = "User not logged in";
         return;
       }
       await _communityService.RSVPToEventAsync(userId.Value, eventId, status);
       await ReloadEventsAsync();
       ErrorMessage = "RSVP submitted successfully!";
       
       // Notify UI to show My RSVPs tab
       OnRsvpSuccessful?.Invoke();
     }
     catch (Exception ex)
     {
       ErrorMessage = $"Failed to RSVP: {ex.Message}";
     }
     finally
     {
       IsLoading = false;
     }
   }

   private async Task OnCancelRsvpAsync(Guid eventId)
   {
     ErrorMessage = string.Empty;
     IsLoading = true;
     try
     {
       var userId = _sessionService.CurrentUser?.Id;
       if (userId == null)
       {
         ErrorMessage = "User not logged in";
         return;
       }
       // You may need to add a CancelRSVPAsync method to ICommunityService
       // For now, we'll treat canceling as changing status to Cancelled
       await _communityService.RSVPToEventAsync(userId.Value, eventId, RSVPStatus.Cancelled);
       await ReloadEventsAsync();
       ErrorMessage = "RSVP cancelled successfully!";
     }
     catch (Exception ex)
     {
       ErrorMessage = $"Failed to cancel RSVP: {ex.Message}";
     }
     finally
     {
       IsLoading = false;
     }
   }

   public async Task CancelRsvpAsync(Guid eventId)
   {
     await OnCancelRsvpAsync(eventId);
   }

    public async Task CancelRsvpDirectAsync(Guid eventId)
    {
      System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] *** CancelRsvpDirectAsync called with eventId: {eventId} ***");
      
      try
      {
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Setting IsLoading = true");
        IsLoading = true;
        ErrorMessage = string.Empty;

        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Getting current user");
        var userId = _sessionService.CurrentUser?.Id;
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Current user ID: {userId}");
        
        if (userId == null)
        {
          System.Diagnostics.Debug.WriteLine("[VIEWMODEL] ERROR: Current user is null!");
          ErrorMessage = "User not logged in";
          return;
        }

        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] About to call RSVPToEventAsync");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Parameters: userId={userId}, eventId={eventId}, status=Cancelled");
        
        // Call the service to cancel the RSVP
        var result = await _communityService.RSVPToEventAsync(userId.Value, eventId, RSVPStatus.Cancelled);
        
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] RSVPToEventAsync completed successfully");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Result: id={result.Id}, status={result.RSVPStatus}, eventId={result.EventId}");

        // Reload events to update the UI
        System.Diagnostics.Debug.WriteLine("[VIEWMODEL] About to call ReloadEventsAsync");
        await ReloadEventsAsync();
        
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] ReloadEventsAsync completed");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] MyRsvpEvents count: {MyRsvpEvents.Count}");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Events count: {Events.Count}");
        
        ErrorMessage = "RSVP cancelled successfully!";
        System.Diagnostics.Debug.WriteLine("[VIEWMODEL] Success message set");
      }
      catch (Exception ex)
      {
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] *** EXCEPTION in CancelRsvpDirectAsync ***");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Exception: {ex}");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Exception Message: {ex.Message}");
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Exception StackTrace: {ex.StackTrace}");
        ErrorMessage = $"Error: {ex.Message}";
      }
      finally
      {
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] Setting IsLoading = false");
        IsLoading = false;
        System.Diagnostics.Debug.WriteLine($"[VIEWMODEL] CancelRsvpDirectAsync finished");
      }
    }
  }

   public class CommunityEventItem
   {
     public Event Event { get; set; } = new();
     public bool IsRsvped { get; set; }

     public Guid EventId => Event.Id;
     public string Name => Event.Name;
     public string Description => Event.Description;
     public DateTime EventDate => Event.EventDate;
     public string Location => Event.Location;
     public int AttendeeCount => Event.CurrentAttendees;
     public string RsvpButtonText => IsRsvped ? "RSVP Submitted" : "RSVP Attending";
     public bool CanRsvp => !IsRsvped;
   }
