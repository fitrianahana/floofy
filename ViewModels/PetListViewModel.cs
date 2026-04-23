using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Services;
using System.Windows.Input;

namespace floofy.ViewModels;

public class PetListViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private ObservableCollection<Pet> _pets = new();
  private string _searchQuery = string.Empty;
  private Pet? _selectedPet;

  public ObservableCollection<Pet> Pets
  {
    get => _pets;
    set => SetProperty(ref _pets, value);
  }

  public string SearchQuery
  {
    get => _searchQuery;
    set => SetProperty(ref _searchQuery, value);
  }

  public Pet? SelectedPet
  {
    get => _selectedPet;
    set => SetProperty(ref _selectedPet, value);
  }

  public ICommand LoadPetsCommand { get; }
  public ICommand SearchPetsCommand { get; }

  public PetListViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    LoadPetsCommand = new RelayCommand(async () => await OnLoadPetsAsync());
    SearchPetsCommand = new RelayCommand(async () => await OnSearchPetsAsync());
  }

  private async Task OnLoadPetsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var pets = await _petService.GetAllPetsAsync();
      Pets = new ObservableCollection<Pet>(pets);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load pets: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnSearchPetsAsync()
  {
    if (string.IsNullOrWhiteSpace(SearchQuery))
    {
      await OnLoadPetsAsync();
      return;
    }
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var pets = await _petService.SearchPetsAsync(SearchQuery);
      Pets = new ObservableCollection<Pet>(pets);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Search failed: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
}