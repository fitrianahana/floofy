namespace floofy.Services;

using floofy.Models;
using floofy.Models.Enums;

public interface ICommunityService
{
  Task<Post> GetPostByIdAsync(Guid postId);
  Task<List<Post>> GetAllPostsAsync();
  Task<List<Post>> GetUserPostsAsync(Guid userId);
  Task<Post> CreatePostAsync(Guid userId, string title, string content, PostVisibility visibility);
  Task<Event> GetEventByIdAsync(Guid eventId);
  Task<List<Event>> GetAllEventsAsync();
  Task<EventRSVP> RSVPToEventAsync(Guid userId, Guid eventId, RSVPStatus status);
}