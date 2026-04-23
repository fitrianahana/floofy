namespace floofy.Data;

using SQLite;
using floofy.Models;
using floofy.Models.Enums;

public class AppDatabase
{
  private const string DatabaseFileName = "floofy.db3";
  private SQLiteAsyncConnection? _database;

  public SQLiteAsyncConnection Database
  {
    get
    {
      if (_database == null)
        throw new InvalidOperationException("Database not initialized");
      return _database;
    }
  }

  public async Task InitializeAsync()
  {
    // get the database file path using MAUI standard location
    string dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFileName);

    // create connection
    _database = new SQLiteAsyncConnection(dbPath);

    // create tables for all entities (in order of dependencies)
    await CreateTablesAsync();
  }

  private async Task CreateTablesAsync()
  {
    // Users
    await _database!.CreateTableAsync<User>();
    await _database.CreateTableAsync<Address>();
    await _database.CreateTableAsync<BankAccount>();
    await _database.CreateTableAsync<PaymentMethod>();

    // Shopping Cart
    await _database.CreateTableAsync<Cart>();
    await _database.CreateTableAsync<CartItem>();

    // Categories
    await _database.CreateTableAsync<PetCategory>();
    await _database.CreateTableAsync<ServiceCategory>();

    // Products 
    await _database.CreateTableAsync<Product>();
    await _database.CreateTableAsync<PetAccessory>();

    // Pets
    await _database.CreateTableAsync<Pet>();
    await _database.CreateTableAsync<PetListing>();

    // Product listings
    await _database.CreateTableAsync<ProductListing>();

    // Services
    await _database.CreateTableAsync<ServicePackage>();
    await _database.CreateTableAsync<ServiceBooking>();

    // Orders
    await _database.CreateTableAsync<Order>();
    await _database.CreateTableAsync<OrderLineItem>();
    await _database.CreateTableAsync<OrderHistory>();

    // Shipping
    await _database.CreateTableAsync<ShippingMethod>();
    await _database.CreateTableAsync<Shipment>();

    // Reviews
    await _database.CreateTableAsync<Review>();

    // Community
    await _database.CreateTableAsync<Post>();
    await _database.CreateTableAsync<Event>();
    await _database.CreateTableAsync<EventRSVP>();

    // Reports
    await _database.CreateTableAsync<SalesReport>();
  }

  public async Task SeedDataIfEmptyAsync()
  {
    // Check if database already has data - only seed if empty
    var userCount = await _database!.Table<User>().CountAsync();
    if (userCount > 0)
    {
      System.Diagnostics.Debug.WriteLine("✅ Database already seeded, skipping mock data");
      return;
    }
    System.Diagnostics.Debug.WriteLine("🌱 Seeding mock data...");
    try
    {
      // Create repositories for inserting data
      var userRepo = new Repository<User>(this);
      var petCategoryRepo = new Repository<PetCategory>(this);
      var petRepo = new Repository<Pet>(this);
      var petListingRepo = new Repository<PetListing>(this);
      var productRepo = new Repository<Product>(this);
      var productListingRepo = new Repository<ProductListing>(this);
      var serviceCategoryRepo = new Repository<ServiceCategory>(this);
      var servicePackageRepo = new Repository<ServicePackage>(this);
      var shippingMethodRepo = new Repository<ShippingMethod>(this);
      var orderRepo = new Repository<Order>(this);
      var orderLineItemRepo = new Repository<OrderLineItem>(this);
      var postRepo = new Repository<Post>(this);
      var eventRepo = new Repository<Event>(this);
      var eventRsvpRepo = new Repository<EventRSVP>(this);
      // ===== 1. SEED FLOOFY SYSTEM USER =====
      var floofyUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      var floofyUser = new User
      {
        Id = floofyUserId,
        FullName = "Floofy Services",
        Email = "system@floofy.com",
        Password = "admin",
        PhoneNumber = "+1-800-FLOOFY",
        ProfileImageUrl = "",
        Roles = new List<RoleType> { RoleType.Seller },
        IsVerified = true,
        VerificationToken = null
      };
      await userRepo.InsertAsync(floofyUser);
      // ===== 2. SEED REGULAR USERS =====
      var user1Id = Guid.NewGuid();
      var user2Id = Guid.NewGuid();
      var user3Id = Guid.NewGuid();
      var user1 = new User
      {
        Id = user1Id,
        FullName = "Frans Jesky",
        Email = "frans@floofy.com",
        Password = "admin",
        PhoneNumber = "+1-555-0001",
        ProfileImageUrl = "",
        Roles = new List<RoleType> { RoleType.Buyer, RoleType.Seller },
        IsVerified = true
      };
      var user2 = new User
      {
        Id = user2Id,
        FullName = "John Seller",
        Email = "john@example.com",
        Password = "admin",
        PhoneNumber = "+1-555-0002",
        ProfileImageUrl = "",
        Roles = new List<RoleType> { RoleType.Seller },
        IsVerified = true
      };
      var user3 = new User
      {
        Id = user3Id,
        FullName = "Jane Buyer",
        Email = "jane@example.com",
        Password = "admin",
        PhoneNumber = "+1-555-0003",
        ProfileImageUrl = "",
        Roles = new List<RoleType> { RoleType.Buyer },
        IsVerified = true
      };
      await userRepo.InsertAllAsync(new[] { user1, user2, user3 });
      // ===== 3. SEED PET CATEGORIES =====
      var petCat1Id = Guid.NewGuid();
      var petCat2Id = Guid.NewGuid();
      var petCat3Id = Guid.NewGuid();
      var petCategories = new[]
      {
      new PetCategory { Id = petCat1Id, Name = "Dogs", Description = "Cute dogs of all breeds", IconUrl = "" },
      new PetCategory { Id = petCat2Id, Name = "Cats", Description = "Adorable cats and kittens", IconUrl = "" },
      new PetCategory { Id = petCat3Id, Name = "Rabbits", Description = "Fluffy rabbits", IconUrl = "" }
    };
      await petCategoryRepo.InsertAllAsync(petCategories);
      // ===== 4. SEED PETS =====
      var pet1Id = Guid.NewGuid();
      var pet2Id = Guid.NewGuid();
      var pet3Id = Guid.NewGuid();
      var pets = new[]
      {
      new Pet
      {
        Id = pet1Id,
        Name = "Max",
        Species = "Dog",
        Breed = "Golden Retriever",
        Age = 2,
        Gender = Gender.Male,
        Weight = 30m,
        Height = 55m,
        Description = "Friendly and energetic golden retriever looking for a loving home",
        ImageUrls = new List<string>(),
        Vaccinated = true,
        Neutered = true,
        HealthCertificate = "VET123",
        SellerId = user1Id,
        PetCategoryId = petCat1Id
      },
      new Pet
      {
        Id = pet2Id,
        Name = "Whiskers",
        Species = "Cat",
        Breed = "Persian",
        Age = 1,
        Gender = Gender.Female,
        Weight = 4m,
        Height = 25m,
        Description = "Soft and gentle Persian cat, perfect companion",
        ImageUrls = new List<string>(),
        Vaccinated = true,
        Neutered = false,
        HealthCertificate = "VET456",
        SellerId = user2Id,
        PetCategoryId = petCat2Id
      },
      new Pet
      {
        Id = pet3Id,
        Name = "Fluffy",
        Species = "Rabbit",
        Breed = "Holland Lop",
        Age = 1,
        Gender = Gender.Female,
        Weight = 2m,
        Height = 20m,
        Description = "Sweet little rabbit ready to hop into your heart",
        ImageUrls = new List<string>(),
        Vaccinated = true,
        Neutered = true,
        HealthCertificate = "VET789",
        SellerId = user1Id,
        PetCategoryId = petCat3Id
      }
    };
      await petRepo.InsertAllAsync(pets);
      // ===== 5. SEED PET LISTINGS =====
      var petListing1Id = Guid.NewGuid();
      var petListing2Id = Guid.NewGuid();
      var petListing3Id = Guid.NewGuid();
      var petListings = new[]
      {
      new PetListing
      {
        Id = petListing1Id,
        Price = 500m,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-5),
        ListingEndDate = null,
        Views = 150,
        PetId = pet1Id,
        SellerId = user1Id
      },
      new PetListing
      {
        Id = petListing2Id,
        Price = 300m,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-3),
        ListingEndDate = null,
        Views = 85,
        PetId = pet2Id,
        SellerId = user2Id
      },
      new PetListing
      {
        Id = petListing3Id,
        Price = 200m,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-2),
        ListingEndDate = null,
        Views = 45,
        PetId = pet3Id,
        SellerId = user1Id
      }
    };
      await petListingRepo.InsertAllAsync(petListings);
      // ===== 6. SEED PRODUCT CATEGORIES =====
      var prodCat1Id = Guid.NewGuid();
      var prodCat2Id = Guid.NewGuid();
      // Note: ProductCategory is an enum, not a table - skip this section
      // Products reference ProductCategoryId as enum value
      // ===== 7. SEED PRODUCTS =====
      var prod1Id = Guid.NewGuid();
      var prod2Id = Guid.NewGuid();
      var prod3Id = Guid.NewGuid();
      var prod4Id = Guid.NewGuid();
      var products = new[]
      {
      new Product
      {
        Id = prod1Id,
        Name = "Premium Pet Food",
        Description = "High-quality dog food with all essential nutrients",
        Price = 45m,
        StockQuantity = 50,
        Sku = "PET-FOOD-001",
        ImageUrls = new List<string>(),
        IsActive = true,
        Discount = 10m,
        Rating = 5,
        SellerId = user1Id,
        ProductCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000001") // PetFood enum as Guid
      },
      new Product
      {
        Id = prod2Id,
        Name = "Comfortable Dog Bed",
        Description = "Soft and cozy bed for your furry friend",
        Price = 89.99m,
        StockQuantity = 30,
        Sku = "PET-BED-001",
        ImageUrls = new List<string>(),
        IsActive = true,
        Discount = 5m,
        Rating = 4,
        SellerId = user2Id,
        ProductCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000003") // PetBedding enum as Guid
      },
      new Product
      {
        Id = prod3Id,
        Name = "Interactive Toy Ball",
        Description = "Fun and engaging toy for active pets",
        Price = 25m,
        StockQuantity = 100,
        Sku = "PET-TOY-001",
        ImageUrls = new List<string>(),
        IsActive = true,
        Discount = 0m,
        Rating = 4,
        SellerId = user1Id,
        ProductCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000002") // PetToys enum as Guid
      },
      new Product
      {
        Id = prod4Id,
        Name = "Pet First Aid Kit",
        Description = "Complete health and wellness kit",
        Price = 65m,
        StockQuantity = 20,
        Sku = "PET-HEALTH-001",
        ImageUrls = new List<string>(),
        IsActive = true,
        Discount = 15m,
        Rating = 5,
        SellerId = user2Id,
        ProductCategoryId = Guid.Parse("00000000-0000-0000-0000-000000000006") // PetHealthcare enum as Guid
      }
    };
      await productRepo.InsertAllAsync(products);
      // ===== 8. SEED PRODUCT LISTINGS =====
      var prodListing1Id = Guid.NewGuid();
      var prodListing2Id = Guid.NewGuid();
      var prodListing3Id = Guid.NewGuid();
      var prodListing4Id = Guid.NewGuid();
      var productListings = new[]
      {
      new ProductListing
      {
        Id = prodListing1Id,
        ListingPrice = 40.50m,
        QuantityListed = 50,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-10),
        ListingEndDate = null,
        Views = 200,
        ProductId = prod1Id,
        SellerId = user1Id
      },
      new ProductListing
      {
        Id = prodListing2Id,
        ListingPrice = 85.49m,
        QuantityListed = 30,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-7),
        ListingEndDate = null,
        Views = 120,
        ProductId = prod2Id,
        SellerId = user2Id
      },
      new ProductListing
      {
        Id = prodListing3Id,
        ListingPrice = 25m,
        QuantityListed = 100,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-5),
        ListingEndDate = null,
        Views = 350,
        ProductId = prod3Id,
        SellerId = user1Id
      },
      new ProductListing
      {
        Id = prodListing4Id,
        ListingPrice = 55.25m,
        QuantityListed = 20,
        IsActive = true,
        ListingStartDate = DateTime.UtcNow.AddDays(-3),
        ListingEndDate = null,
        Views = 80,
        ProductId = prod4Id,
        SellerId = user2Id
      }
    };
      await productListingRepo.InsertAllAsync(productListings);
      // ===== 9. SEED SERVICE CATEGORIES =====
      var servCat1Id = Guid.NewGuid();
      var servCat2Id = Guid.NewGuid();
      var serviceCategories = new[]
      {
      new ServiceCategory { Id = servCat1Id, Name = "Grooming", Description = "Professional pet grooming services", IconUrl = "" },
      new ServiceCategory { Id = servCat2Id, Name = "Training", Description = "Expert pet training services", IconUrl = "" }
    };
      await serviceCategoryRepo.InsertAllAsync(serviceCategories);
      // ===== 10. SEED SERVICE PACKAGES =====
      var svc1Id = Guid.NewGuid();
      var svc2Id = Guid.NewGuid();
      var servicePackages = new[]
      {
      new ServicePackage
      {
        Id = svc1Id,
        Name = "Professional Grooming",
        Description = "Complete grooming service including bath, trim, and styling",
        Price = 75m,
        Duration = 120,
        MaxCapacity = 5,
        CurrentBookings = 0,
        IsActive = true,
        ServiceCategoryId = servCat1Id,
        SellerId = floofyUserId,
        StockQuantity = 5,
        Sku = "SVC-GROOM-001",
        ImageUrls = new List<string>(),
        Discount = 0m,
        Rating = 5
      },
      new ServicePackage
      {
        Id = svc2Id,
        Name = "Basic Obedience Training",
        Description = "4-week obedience training program for dogs",
        Price = 150m,
        Duration = 4 * 7 * 24 * 60,
        MaxCapacity = 10,
        CurrentBookings = 0,
        IsActive = true,
        ServiceCategoryId = servCat2Id,
        SellerId = floofyUserId,
        StockQuantity = 10,
        Sku = "SVC-TRAIN-001",
        ImageUrls = new List<string>(),
        Discount = 10m,
        Rating = 4
      }
    };
      await servicePackageRepo.InsertAllAsync(servicePackages);
      // ===== 11. SEED SHIPPING METHODS =====
      var shippingId = Guid.NewGuid();
      var shippingMethod = new ShippingMethod
      {
        Id = shippingId,
        Name = "Standard Shipping",
        Description = "Free standard shipping for orders over $50",
        BaseCost = 5.99m,
        EstimatedDays = 5,
        IsActive = true
      };
      await shippingMethodRepo.InsertAsync(shippingMethod);
      // ===== 12. SEED SAMPLE ORDER =====
      var orderId = Guid.NewGuid();
      var orderLineItemId = Guid.NewGuid();
      var orderHistoryId = Guid.NewGuid();
      var order = new Order
      {
        Id = orderId,
        OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-001",
        OrderDate = DateTime.UtcNow.AddDays(-5),
        TotalPrice = 40.50m,
        TotalTax = 3.24m,
        TotalDiscount = 4.05m,
        FinalPrice = 39.69m,
        Status = OrderStatus.Delivered,
        PaymentStatus = PaymentStatus.Completed,
        ShippingStatus = ShippingStatus.Delivered,
        Notes = "Sample order for demonstration",
        BuyerId = user3Id,
        ShippingAddressId = Guid.NewGuid(),
        BillingAddressId = Guid.NewGuid(),
        ShippingMethodId = shippingId,
        PaymentMethodId = null
      };
      await orderRepo.InsertAsync(order);
      var orderLineItem = new OrderLineItem
      {
        Id = orderLineItemId,
        Quantity = 1,
        UnitPrice = 45m,
        TotalPrice = 40.50m,
        Discount = 4.05m,
        ProductId = prod1Id,
        OrderId = orderId
      };
      await orderLineItemRepo.InsertAsync(orderLineItem);
      var orderHistory = new OrderHistory
      {
        Id = orderHistoryId,
        PreviousStatus = OrderStatus.Pending,
        NewStatus = OrderStatus.Delivered,
        Timestamp = DateTime.UtcNow.AddDays(-1),
        Notes = "Order delivered successfully",
        OrderId = orderId
      };
      var orderHistoryRepo = new Repository<OrderHistory>(this);
      await orderHistoryRepo.InsertAsync(orderHistory);
      // ===== 13. SEED POSTS =====
      var post1Id = Guid.NewGuid();
      var post2Id = Guid.NewGuid();
      var posts = new[]
      {
      new Post
      {
        Id = post1Id,
        Title = "My new puppy Max is here!",
        Content = "Just brought home my beautiful Golden Retriever puppy. He's so energetic and playful!",
        ImageUrls = new List<string>(),
        LikeCount = 23,
        CommentCount = 5,
        Visibility = PostVisibility.Public,
        AuthorId = user3Id
      },
      new Post
      {
        Id = post2Id,
        Title = "Pet training tips for beginners",
        Content = "Here are some basic training tips that worked great for my dogs. Start with positive reinforcement!",
        ImageUrls = new List<string>(),
        LikeCount = 45,
        CommentCount = 12,
        Visibility = PostVisibility.Public,
        AuthorId = user1Id
      }
    };
      await postRepo.InsertAllAsync(posts);
      // ===== 14. SEED EVENTS =====
      var event1Id = Guid.NewGuid();
      var evt = new Event
      {
        Id = event1Id,
        Name = "Pet Lovers Meetup",
        Description = "Monthly meetup for pet enthusiasts. Bring your pets and meet fellow pet lovers!",
        EventDate = DateTime.UtcNow.AddDays(14),
        Location = "Central Park, City",
        MaxAttendees = 50,
        CurrentAttendees = 12,
        ImageUrl = "",
        OrganizerId = user1Id
      };
      await eventRepo.InsertAsync(evt);
      // ===== 15. SEED EVENT RSVPs =====
      var rsvp1Id = Guid.NewGuid();
      var eventRsvp = new EventRSVP
      {
        Id = rsvp1Id,
        RSVPStatus = RSVPStatus.Attending,
        RegistrationDate = DateTime.UtcNow.AddDays(-3),
        AttendeeId = user2Id,
        EventId = event1Id
      };
      await eventRsvpRepo.InsertAsync(eventRsvp);
      System.Diagnostics.Debug.WriteLine("✅ Mock data seeding completed successfully!");
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"❌ Error seeding mock data: {ex.Message}");
      throw;
    }
  }
}