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

  private ObservableCollection<Post> _posts = new();
  private ObservableCollection<Event> _events = new();

  private Post? _selectedPost;
  private Event? _selectedEvent;
  private string _newPostTitle = string.Empty;
  private string _newPostContent = string.Empty;
  private PostVisibility _postVisibility = PostVisibility.Public;

  public ICommand RSVPAttendingCommand { get; }

  public ObservableCollection<Post> Posts
  {
    get => _posts;
    set => SetProperty(ref _posts, value);
  }

  public ObservableCollection<Event> Events
  {
    get => _events;
    set => SetProperty(ref _events, value);
  }

  public Post? SelectedPost
  {
    get => _selectedPost;
    set => SetProperty(ref _selectedPost, value);
  }

  public Event? SelectedEvent
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

  public CommunityViewModel()
  {
    _communityService = App.Services.GetRequiredService<ICommunityService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    LoadPostsCommand = new RelayCommand(async () => await OnLoadPostsAsync());
    LoadEventsCommand = new RelayCommand(async () => await OnLoadEventsAsync());
    CreatePostCommand = new RelayCommand(async () => await OnCreatePostAsync());
    RSVPToEventCommand = new RelayCommand<(Guid, RSVPStatus)>(async (param) => await OnRSVPToEventAsync(param.Item1, param.Item2));
    RSVPAttendingCommand = new RelayCommand<Guid>(async (eventId) => await OnRSVPToEventAsync(eventId, RSVPStatus.Attending));
  }

  private async Task OnLoadPostsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var posts = await _communityService.GetAllPostsAsync();
      Posts = new ObservableCollection<Post>(posts);
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
      var events = await _communityService.GetAllEventsAsync();
      Events = new ObservableCollection<Event>(events);
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
      await OnLoadPostsAsync();
      ErrorMessage = "Post created successfully!";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to create post: {ex.Message}";
      IsLoading = false;
    }
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
      await OnLoadEventsAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to RSVP: {ex.Message}";
      IsLoading = false;
    }
  }
}