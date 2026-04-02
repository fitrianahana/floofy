using floofy.Models;

namespace floofy.Services;

public class SessionService
{
  public User? CurrentUser { get; private set; }

  public bool IsLoggedIn => CurrentUser is not null;
  public void SetCurrentUser(User user) => CurrentUser = user;
  public void Logout() => CurrentUser = null;
}