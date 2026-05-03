using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class PetDetailViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private Pet? _pet;

  public Pet? Pet
  {
    get => _pet;
    set => SetProperty(ref _pet, value);
  }

  public PetDetailViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
  }

  public async Task LoadPetAsync(Guid petId)
  {
    ErrorMessage = string.Empty;
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
}