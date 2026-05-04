namespace floofy.Services;

using floofy.Data;
using floofy.Models;

public class PetService : IPetService
{
  private readonly IRepository<Pet> _petRepository;
  private readonly IRepository<PetListing> _petListingRepository;

  public PetService(
      IRepository<Pet> petRepository,
      IRepository<PetListing> petListingRepository)
  {
    _petRepository = petRepository;
    _petListingRepository = petListingRepository;
  }

  public async Task<Pet> GetPetByIdAsync(Guid petId)
  {
    return (await _petRepository.GetByIdAsync(petId))!;
  }

  public async Task<List<Pet>> GetAllPetsAsync()
  {
    var allPets = await _petRepository.GetAllAsync();
    return allPets.Where(p => !p.IsDeleted).ToList();
  }

  public async Task<List<Pet>> GetPetsByCategoryAsync(Guid categoryId)
  {
    var allPets = await _petRepository.GetAllAsync();
    return allPets
        .Where(p => p.PetCategoryId == categoryId && !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Pet>> SearchPetsAsync(string query)
  {
    var allPets = await _petRepository.GetAllAsync();
    return allPets
        .Where(p => (p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     p.Species.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     p.Breed.Contains(query, StringComparison.OrdinalIgnoreCase)) &&
                    !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Pet>> GetSellerPetsAsync(Guid sellerId)
  {
    var allPets = await _petRepository.GetAllAsync();
    return allPets
        .Where(p => p.SellerId == sellerId && !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Pet>> GetPetsByBreedAsync(string breed)
  {
    var allPets = await _petRepository.GetAllAsync();
    return allPets
        .Where(p => p.Breed.Equals(breed, StringComparison.OrdinalIgnoreCase) && !p.IsDeleted)
        .ToList();
  }

  public async Task<Pet> CreatePetWithListingAsync(Pet pet, decimal listingPrice)
  {
    if (pet.Id == Guid.Empty)
      pet.Id = Guid.NewGuid();

    await _petRepository.InsertAsync(pet);

    var listing = pet.GenerateListing();
    listing.Price = listingPrice;
    await _petListingRepository.InsertAsync(listing);

    return pet;
  }

  public async Task<PetListing?> GetActiveListingForPetAsync(Guid petId)
  {
    var listings = await _petListingRepository.GetAllAsync();
    return listings
        .Where(l => l.PetId == petId && !l.IsDeleted && l.IsActive)
        .OrderByDescending(l => l.ListingStartDate)
        .FirstOrDefault();
  }
}