using System.Windows.Input;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class PetDetailViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly SessionService _sessionService;

  private Pet? _pet;
  private string _statusMessage = string.Empty;
  private bool _isSubmittingAdoption;

  public Pet? Pet
  {
    get => _pet;
    set
    {
      SetProperty(ref _pet, value);
      OnPropertyChanged(nameof(CanAdopt));
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

  public bool CanAdopt => Pet is not null && !_isSubmittingAdoption;

  public ICommand AdoptCommand { get; }

  public PetDetailViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    AdoptCommand = new RelayCommand(OnAdopt);
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
      }
      else
      {
        Pet = pet;
      }
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

  private void OnAdopt()
  {
    if (Pet is null) return;

    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to start an adoption.";
      return;
    }

    ErrorMessage = string.Empty;
    IsSubmittingAdoption = true;

    try
    {
      StatusMessage = $"Adoption request sent for {Pet.Name}. We'll be in touch soon!";
    }
    finally
    {
      IsSubmittingAdoption = false;
    }
  }
}