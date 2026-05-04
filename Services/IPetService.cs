namespace floofy.Services;

using floofy.Models;

public interface IPetService
{
  Task<Pet> GetPetByIdAsync(Guid petId);
  Task<List<Pet>> GetAllPetsAsync();
  Task<List<Pet>> GetPetsByCategoryAsync(Guid categoryId);
  Task<List<Pet>> SearchPetsAsync(string query);
  Task<List<Pet>> GetSellerPetsAsync(Guid sellerId);
  Task<List<Pet>> GetPetsByBreedAsync(string breed);
  Task<Pet> CreatePetWithListingAsync(Pet pet, decimal listingPrice);
  Task<PetListing?> GetActiveListingForPetAsync(Guid petId);
  Task CancelListingAsync(Guid petId);
}