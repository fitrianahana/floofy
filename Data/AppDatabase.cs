namespace floofy.Data;

using SQLite;
using floofy.Models;

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
    await _database.CreateTableAsync<User>();
    await _database.CreateTableAsync<Address>();
    await _database.CreateTableAsync<BankAccount>();
    await _database.CreateTableAsync<PaymentMethod>();

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
}