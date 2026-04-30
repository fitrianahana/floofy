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

  public async Task<List<EventRSVP>> GetUserEventRSVPsAsync(Guid userId)
  {
    System.Diagnostics.Debug.WriteLine($"[SERVICE] GetUserEventRSVPsAsync called for userId: {userId}");
    var allRsvps = await _eventRSVPRepository.GetAllAsync();
    System.Diagnostics.Debug.WriteLine($"[SERVICE] Total RSVPs in database: {allRsvps.Count}");
    
    var result = allRsvps
        .Where(r => r.AttendeeId == userId && !r.IsDeleted)
        .ToList();
    
    System.Diagnostics.Debug.WriteLine($"[SERVICE] Returning {result.Count} RSVPs for user {userId}");
    foreach (var rsvp in result)
    {
      System.Diagnostics.Debug.WriteLine($"[SERVICE]   - RSVP ID: {rsvp.Id}, EventId: {rsvp.EventId}, Status: {rsvp.RSVPStatus}");
    }
    
    return result;
  }

  public async Task<EventRSVP> RSVPToEventAsync(Guid userId, Guid eventId, RSVPStatus status)
  {
    System.Diagnostics.Debug.WriteLine($"[SERVICE] RSVPToEventAsync called: userId={userId}, eventId={eventId}, status={status}");
    
    var allRsvps = await _eventRSVPRepository.GetAllAsync();
    System.Diagnostics.Debug.WriteLine($"[SERVICE] Retrieved {allRsvps.Count} RSVPs from database");
    
    var existing = allRsvps.FirstOrDefault(r => r.AttendeeId == userId && r.EventId == eventId && !r.IsDeleted);
    System.Diagnostics.Debug.WriteLine($"[SERVICE] Found existing RSVP: {(existing != null ? "YES" : "NO")}");

    if (existing != null)
    {
      System.Diagnostics.Debug.WriteLine($"[SERVICE] Updating existing RSVP");
      System.Diagnostics.Debug.WriteLine($"[SERVICE] Old status: {existing.RSVPStatus}, New status: {status}");
      existing.RSVPStatus = status;
      existing.RegistrationDate = DateTime.UtcNow;
      System.Diagnostics.Debug.WriteLine($"[SERVICE] About to call UpdateAsync");
      await _eventRSVPRepository.UpdateAsync(existing);
      System.Diagnostics.Debug.WriteLine($"[SERVICE] UpdateAsync completed");
      System.Diagnostics.Debug.WriteLine($"[SERVICE] RSVP after update - Status: {existing.RSVPStatus}, UpdatedAt: {existing.UpdatedAt}");
      return existing;
    }

    System.Diagnostics.Debug.WriteLine($"[SERVICE] Creating new RSVP");
    var rsvp = new EventRSVP
    {
      AttendeeId = userId,
      EventId = eventId,
      RSVPStatus = status,
      RegistrationDate = DateTime.UtcNow
    };
    await _eventRSVPRepository.InsertAsync(rsvp);
    System.Diagnostics.Debug.WriteLine($"[SERVICE] New RSVP created with ID: {rsvp.Id}");
    return rsvp;
  }
}
