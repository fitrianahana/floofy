using System.Windows.Input;
using floofy.Models;
using floofy.Models.Enums;
using floofy.Services;

namespace floofy.ViewModels;

public class SellPetViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly SessionService _sessionService;

  private string _name = string.Empty;
  private string _species = string.Empty;
  private string _breed = string.Empty;
  private int _age = 1;
  private Gender _gender = Gender.Male;
  private decimal _weight;
  private decimal _height;
  private string _description = string.Empty;
  private string _thumbnail = string.Empty;
  private bool _vaccinated;
  private bool _neutered;
  private string _healthCertificate = string.Empty;
  private decimal _price;
  private string _statusMessage = string.Empty;

  public string Name { get => _name; set => SetProperty(ref _name, value); }
  public string Species { get => _species; set => SetProperty(ref _species, value); }
  public string Breed { get => _breed; set => SetProperty(ref _breed, value); }
  public int Age { get => _age; set => SetProperty(ref _age, value); }
  public Gender Gender { get => _gender; set => SetProperty(ref _gender, value); }
  public decimal Weight { get => _weight; set => SetProperty(ref _weight, value); }
  public decimal Height { get => _height; set => SetProperty(ref _height, value); }
  public string Description { get => _description; set => SetProperty(ref _description, value); }
  public string Thumbnail { get => _thumbnail; set => SetProperty(ref _thumbnail, value); }
  public bool Vaccinated { get => _vaccinated; set => SetProperty(ref _vaccinated, value); }
  public bool Neutered { get => _neutered; set => SetProperty(ref _neutered, value); }
  public string HealthCertificate { get => _healthCertificate; set => SetProperty(ref _healthCertificate, value); }
  public decimal Price { get => _price; set => SetProperty(ref _price, value); }

  public string StatusMessage
  {
    get => _statusMessage;
    set => SetProperty(ref _statusMessage, value);
  }

  public List<Gender> GenderOptions { get; } = new() { Gender.Male, Gender.Female };

  public ICommand SubmitCommand { get; }

  public SellPetViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();

    SubmitCommand = new RelayCommand(async () => await OnSubmitAsync());
  }

  public Func<Task>? OnSubmitted { get; set; }

  private async Task OnSubmitAsync()
  {
    ErrorMessage = string.Empty;
    StatusMessage = string.Empty;

    var user = _sessionService.CurrentUser;
    if (user is null)
    {
      ErrorMessage = "Please sign in to list a pet for rehoming.";
      return;
    }

    if (!user.IsSeller)
    {
      ErrorMessage = "Only rehomers can list pets.";
      return;
    }

    if (string.IsNullOrWhiteSpace(Name) ||
        string.IsNullOrWhiteSpace(Species) ||
        string.IsNullOrWhiteSpace(Breed) ||
        string.IsNullOrWhiteSpace(Description))
    {
      ErrorMessage = "Name, species, breed, and description are required.";
      return;
    }

    if (Age < 0 || Weight < 0 || Height < 0 || Price < 0)
    {
      ErrorMessage = "Age, weight, height, and price cannot be negative.";
      return;
    }

    IsLoading = true;
    try
    {
      var pet = new Pet
      {
        Id = Guid.NewGuid(),
        Name = Name.Trim(),
        Species = Species.Trim(),
        Breed = Breed.Trim(),
        Age = Age,
        Gender = Gender,
        Weight = Weight,
        Height = Height,
        Description = Description.Trim(),
        Thumbnail = Thumbnail.Trim(),
        Vaccinated = Vaccinated,
        Neutered = Neutered,
        HealthCertificate = string.IsNullOrWhiteSpace(HealthCertificate) ? null : HealthCertificate.Trim(),
        SellerId = user.Id,
        PetCategoryId = Guid.Empty
      };

      await _petService.CreatePetWithListingAsync(pet, Price);

      StatusMessage = $"{pet.Name} is now listed for rehoming!";

      if (OnSubmitted is not null)
      {
        await OnSubmitted();
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to list pet for rehoming: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  public void Reset()
  {
    Name = string.Empty;
    Species = string.Empty;
    Breed = string.Empty;
    Age = 1;
    Gender = Gender.Male;
    Weight = 0;
    Height = 0;
    Description = string.Empty;
    Thumbnail = string.Empty;
    Vaccinated = false;
    Neutered = false;
    HealthCertificate = string.Empty;
    Price = 0;
    StatusMessage = string.Empty;
    ErrorMessage = string.Empty;
  }
}
