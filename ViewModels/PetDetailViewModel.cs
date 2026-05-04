using System.Windows.Input;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class PetDetailViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly ICartService _cartService;
  private readonly SessionService _sessionService;

  private Pet? _pet;
  private decimal _adoptionFee;
  private bool _isInCart;
  private string _statusMessage = string.Empty;
  private bool _isSubmittingAdoption;
  private bool _isOwner;

  public Pet? Pet
  {
    get => _pet;
    set
    {
      SetProperty(ref _pet, value);
      OnPropertyChanged(nameof(CanAdopt));
      OnPropertyChanged(nameof(IsOwner));
    }
  }

  public decimal AdoptionFee
  {
    get => _adoptionFee;
    set => SetProperty(ref _adoptionFee, value);
  }

  public bool IsInCart
  {
    get => _isInCart;
    set
    {
      SetProperty(ref _isInCart, value);
      OnPropertyChanged(nameof(CanAdopt));
      OnPropertyChanged(nameof(AdoptButtonText));
    }
  }

  public string StatusMessage
  {
    get => _statusMessage;
    set => SetProperty(ref _statusMessage, value);
  }

  public bool IsSubmittingAdoption
  {
    get => _isSubmittingAdoption;
    set
    {
      SetProperty(ref _isSubmittingAdoption, value);
      OnPropertyChanged(nameof(CanAdopt));
    }
  }

  public bool IsOwner
  {
    get => _isOwner;
    set => SetProperty(ref _isOwner, value);
  }

  public bool CanAdopt => Pet is not null && !_isSubmittingAdoption && !_isInCart && !IsOwner;
  public string AdoptButtonText => _isInCart ? "In Cart" : "Adopt Pet";

  public ICommand AdoptCommand { get; }
  public ICommand CancelListingCommand { get; }

  public PetDetailViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _cartService = App.Services.GetRequiredService<ICartService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    AdoptCommand = new RelayCommand(async () => await OnAdoptAsync());
    CancelListingCommand = new RelayCommand(async () => await OnCancelListingAsync());
  }

  public async Task LoadPetAsync(Guid petId)
  {
    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsLoading = true;
    try
    {
      var pet = await _petService.GetPetByIdAsync(petId);
      if (pet == null)
      {
        ErrorMessage = "Pet not found.";
        Pet = null;
        return;
      }

      Pet = pet;

      var listing = await _petService.GetActiveListingForPetAsync(pet.Id);
      AdoptionFee = listing?.Price ?? 0m;

      var user = _sessionService.CurrentUser;
      IsInCart = user is not null && await _cartService.IsPetInCartAsync(user.Id, pet.Id);
      IsOwner = user is not null && pet.SellerId == user.Id;
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load pet: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnAdoptAsync()
  {
    if (Pet is null) return;

    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to start an adoption.";
      return;
    }

    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;
    IsSubmittingAdoption = true;

    try
    {
      var added = await _cartService.AddPetToCartAsync(user.Id, Pet.Id, AdoptionFee);
      if (added)
      {
        StatusMessage = $"{Pet.Name} added to your cart for adoption.";
        IsInCart = true;
      }
      else
      {
        StatusMessage = $"{Pet.Name} is already in your cart.";
        IsInCart = true;
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Could not add to cart: {ex.Message}";
    }
    finally
    {
      IsSubmittingAdoption = false;
    }
  }

  public async Task OnCancelListingAsync()
  {
    if (Pet is null) return;

    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to cancel your listing.";
      return;
    }

    if (Pet.SellerId != user.Id)
    {
      ErrorMessage = "You can only cancel your own listings.";
      return;
    }

    try
    {
      await _petService.CancelListingAsync(Pet.Id);
      StatusMessage = $"{Pet.Name}'s listing has been cancelled.";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to cancel listing: {ex.Message}";
    }
  }
}
