namespace floofy.Services;

using floofy.Models;

public interface IProductService
{
  Task<Product> GetProductByIdAsync(Guid productId);
  Task<List<Product>> GetAllProductsAsync();
  Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId);
  Task<List<Product>> SearchProductsAsync(string query);
  Task<List<Product>> GetSellerProductsAsync(Guid sellerId);
  Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
}