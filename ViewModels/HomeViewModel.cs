using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Services;

namespace floofy.ViewModels;

public class HomeViewModel : BaseViewModel
{
  private readonly IPetService _petService;
  private readonly IProductService _productService;
  private readonly ICartService _cartService;
  private readonly SessionService _sessionService;

  private string _greeting = "Welcome back!";
  private string _userFirstName = string.Empty;
  private string _subtitle = "Discover pets, products, and services";
  private int _cartItemCount;

  private readonly ObservableCollection<Pet> _featuredPets = new();
  private readonly ObservableCollection<Product> _featuredProducts = new();
  private readonly ObservableCollection<HomeArticle> _articles = new();
  private readonly ObservableCollection<HomeReview> _reviews = new();

  public string Greeting
  {
    get => _greeting;
    set => SetProperty(ref _greeting, value);
  }

  public string UserFirstName
  {
    get => _userFirstName;
    set => SetProperty(ref _userFirstName, value);
  }

  public string Subtitle
  {
    get => _subtitle;
    set => SetProperty(ref _subtitle, value);
  }

  public int CartItemCount
  {
    get => _cartItemCount;
    set
    {
      SetProperty(ref _cartItemCount, value);
      OnPropertyChanged(nameof(HasCartItems));
      OnPropertyChanged(nameof(CartBadgeText));
    }
  }

  public bool HasCartItems => _cartItemCount > 0;
  public string CartBadgeText => _cartItemCount > 9 ? "9+" : _cartItemCount.ToString();

  public ObservableCollection<Pet> FeaturedPets => _featuredPets;
  public ObservableCollection<Product> FeaturedProducts => _featuredProducts;
  public ObservableCollection<HomeArticle> Articles => _articles;
  public ObservableCollection<HomeReview> Reviews => _reviews;

  public bool HasFeaturedPets => _featuredPets.Count > 0;
  public bool HasFeaturedProducts => _featuredProducts.Count > 0;
  public bool HasArticles => _articles.Count > 0;
  public bool HasReviews => _reviews.Count > 0;

  public bool IsBuyer => _sessionService.CurrentUser?.IsBuyer ?? false;
  public bool IsSeller => _sessionService.CurrentUser?.IsSeller ?? false;

  public HomeViewModel()
  {
    _petService = App.Services.GetRequiredService<IPetService>();
    _productService = App.Services.GetRequiredService<IProductService>();
    _cartService = App.Services.GetRequiredService<ICartService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
  }

  public async Task LoadAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;

    try
    {
      RefreshGreeting();

      var pets = await _petService.GetAllPetsAsync();
      _featuredPets.Clear();
      foreach (var p in pets.Take(6))
      {
        _featuredPets.Add(p);
      }
      OnPropertyChanged(nameof(HasFeaturedPets));

      var products = await _productService.GetAllProductsAsync();
      _featuredProducts.Clear();
      foreach (var p in products.Take(6))
      {
        _featuredProducts.Add(p);
      }
      OnPropertyChanged(nameof(HasFeaturedProducts));

      LoadSampleArticles();
      LoadSampleReviews();

      var user = _sessionService.CurrentUser;
      if (user is not null)
      {
        var cart = await _cartService.GetUserCartAsync(user.Id);
        var products_count = cart.Items.Sum(i => i.Quantity);
        var pets_count = cart.PetItems.Count;
        CartItemCount = products_count + pets_count;
      }
      else
      {
        CartItemCount = 0;
      }
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load home: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private void LoadSampleArticles()
  {
    if (_articles.Count > 0) return;
    _articles.Add(new HomeArticle
    {
      Title = "5 things to know before adopting your first dog",
      Category = "Adoption",
      Thumbnail = "article_1.jpg",
      ReadTime = "4 min read",
      Author = "Dr. Maya Chen",
      Excerpt = "A friendly guide to setting expectations and preparing your home."
    });
    _articles.Add(new HomeArticle
    {
      Title = "Cat nutrition basics: what's actually in the bag",
      Category = "Nutrition",
      Thumbnail = "article_2.jpg",
      ReadTime = "6 min read",
      Author = "The Floofy Team",
      Excerpt = "Decoding pet food labels so you can pick the right one."
    });
    _articles.Add(new HomeArticle
    {
      Title = "Rainy day enrichment ideas for indoor pets",
      Category = "Enrichment",
      Thumbnail = "article_3.jpg",
      ReadTime = "3 min read",
      Author = "Lina Park",
      Excerpt = "Quick games and puzzles to keep your pet engaged."
    });
    _articles.Add(new HomeArticle
    {
      Title = "Understanding your pet's body language",
      Category = "Behavior",
      Thumbnail = "article_4.jpg",
      ReadTime = "5 min read",
      Author = "Dr. Rafael Ortiz",
      Excerpt = "Subtle cues that tell you how your pet is feeling."
    });
    OnPropertyChanged(nameof(HasArticles));
  }

  private void LoadSampleReviews()
  {
    if (_reviews.Count > 0) return;
    _reviews.Add(new HomeReview
    {
      AuthorName = "Hannah W.",
      Avatar = string.Empty,
      Rating = 5,
      Comment = "Adopted Bella through Floofy and the process was so smooth. The seller was caring and the after-care tips really helped.",
      Subject = "Adoption • Bella",
      TimeAgo = "2 days ago"
    });
    _reviews.Add(new HomeReview
    {
      AuthorName = "Marcus T.",
      Avatar = string.Empty,
      Rating = 4,
      Comment = "Great selection of products and fast checkout. Would love more grain-free options though.",
      Subject = "Shop • Pet Food",
      TimeAgo = "1 week ago"
    });
    _reviews.Add(new HomeReview
    {
      AuthorName = "Priya S.",
      Avatar = string.Empty,
      Rating = 5,
      Comment = "Floofy+ vet booking saved my evening. Friendly clinic and clear pricing upfront.",
      Subject = "Floofy+ • Vet visit",
      TimeAgo = "3 weeks ago"
    });
    OnPropertyChanged(nameof(HasReviews));
  }

  public void RefreshGreeting()
  {
    var hour = DateTime.Now.Hour;
    var timePart = hour switch
    {
      >= 5 and < 12 => "Good morning",
      >= 12 and < 17 => "Good afternoon",
      >= 17 and < 22 => "Good evening",
      _ => "Hi there"
    };

    var user = _sessionService.CurrentUser;
    var first = string.Empty;
    if (user is not null && !string.IsNullOrWhiteSpace(user.FullName))
    {
      first = user.FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty;
    }

    UserFirstName = first;
    Greeting = string.IsNullOrEmpty(first) ? $"{timePart}!" : $"{timePart}, {first}!";

    Subtitle = (IsSeller, IsBuyer) switch
    {
      (true, true) => "Find your next companion or list one for adoption",
      (true, false) => "Help a pet find its forever home today",
      (false, true) => "Find your new best friend",
      _ => "Discover pets, products, and services"
    };

    OnPropertyChanged(nameof(IsBuyer));
    OnPropertyChanged(nameof(IsSeller));
  }
}
