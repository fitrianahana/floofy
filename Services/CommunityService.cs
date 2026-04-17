namespace floofy.Services;

using floofy.Data;
using floofy.Models;
using floofy.Models.Enums;

public class CommunityService : ICommunityService
{
  private readonly IRepository<Post> _postRepository;
  private readonly IRepository<Event> _eventRepository;
  private readonly IRepository<EventRSVP> _eventRSVPRepository;

  public CommunityService(
      IRepository<Post> postRepository,
      IRepository<Event> eventRepository,
      IRepository<EventRSVP> eventRSVPRepository)
  {
    _postRepository = postRepository;
    _eventRepository = eventRepository;
    _eventRSVPRepository = eventRSVPRepository;
  }

  public async Task<Post> GetPostByIdAsync(Guid postId)
  {
    return (await _postRepository.GetByIdAsync(postId))!;
  }

  public async Task<List<Post>> GetAllPostsAsync()
  {
    var allPosts = await _postRepository.GetAllAsync();
    return allPosts.Where(p => !p.IsDeleted).ToList();
  }

  public async Task<List<Post>> GetUserPostsAsync(Guid userId)
  {
    var allPosts = await _postRepository.GetAllAsync();
    return allPosts
        .Where(p => p.AuthorId == userId && !p.IsDeleted)
        .ToList();
  }

  public async Task<Post> CreatePostAsync(Guid userId, string title, string content, PostVisibility visibility)
  {
    var post = new Post
    {
      Title = title,
      Content = content,
      Visibility = visibility,
      AuthorId = userId,
      ImageUrls = new List<string>(),
      LikeCount = 0,
      CommentCount = 0
    };
    await _postRepository.InsertAsync(post);
    return post;
  }

  public async Task<Event> GetEventByIdAsync(Guid eventId)
  {
    return (await _eventRepository.GetByIdAsync(eventId))!;
  }

  public async Task<List<Event>> GetAllEventsAsync()
  {
    var allEvents = await _eventRepository.GetAllAsync();
    return allEvents.Where(e => !e.IsDeleted).ToList();
  }

  public async Task<EventRSVP> RSVPToEventAsync(Guid userId, Guid eventId, RSVPStatus status)
  {
    var rsvp = new EventRSVP
    {
      AttendeeId = userId,
      EventId = eventId,
      RSVPStatus = status,
      RegistrationDate = DateTime.UtcNow
    };
    await _eventRSVPRepository.InsertAsync(rsvp);
    return rsvp;
  }
}