using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class MyListingsViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly SessionService _sessionService;

  private readonly ObservableCollection<Pet> _listings = new();

  public ObservableCollection<Pet> Listings => _listings;

  public bool HasListings => _listings.Count > 0;
  public bool ShowList => !IsLoading && HasListings;
  public bool ShowEmpty => !IsLoading && !HasListings;

  public MyListingsViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    PropertyChanged += (_, e) =>
    {
      if (e.PropertyName == nameof(IsLoading))
      {
        OnPropertyChanged(nameof(ShowList));
        OnPropertyChanged(nameof(ShowEmpty));
      }
    };
  }

  public async Task CancelListingAsync(Pet pet)
  {
    if (pet is null) return;
    try
    {
      await _petService.CancelListingAsync(pet.Id);

      var user = _sessionService.CurrentUser;
      if (user is not null)
      {
        var pets = await _petService.GetSellerPetsAsync(user.Id);
        _listings.Clear();
        foreach (var p in pets)
        {
          _listings.Add(p);
        }
      }
      else
      {
        _listings.Clear();
      }

      OnPropertyChanged(nameof(HasListings));
      OnPropertyChanged(nameof(ShowList));
      OnPropertyChanged(nameof(ShowEmpty));
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Couldn't cancel: {ex.Message}";
    }
  }

  public async Task LoadAsync()
  {
    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to view your listings.";
      _listings.Clear();
      return;
    }

    ErrorMessage = string.Empty;
    IsLoading = true;

    try
    {
      var pets = await _petService.GetSellerPetsAsync(user.Id);
      _listings.Clear();
      foreach (var p in pets)
      {
        _listings.Add(p);
      }
      OnPropertyChanged(nameof(HasListings));
      OnPropertyChanged(nameof(ShowList));
      OnPropertyChanged(nameof(ShowEmpty));
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load listings: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
}
