namespace floofy.Services;

using floofy.Data;
using floofy.Models;
using System.Linq;

public class ProductService : IProductService
{
  private readonly IRepository<Product> _productRepository;

  public ProductService(IRepository<Product> productRepository)
  {
    _productRepository = productRepository;
  }

  public async Task<Product> GetProductByIdAsync(Guid productId)
  {
    return (await _productRepository.GetByIdAsync(productId))!;
  }

  public async Task<List<Product>> GetAllProductsAsync()
  {
    var allProducts = await _productRepository.GetAllAsync();
    return allProducts.Where(p => p.IsActive && !p.IsDeleted).ToList();
  }

  public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId)
  {
    var allProducts = await _productRepository.GetAllAsync();
    return allProducts
        .Where(p => p.ProductCategoryId == categoryId && p.IsActive && !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Product>> SearchProductsAsync(string query)
  {
    var allProducts = await _productRepository.GetAllAsync();
    return allProducts
        .Where(p => (p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                     p.Description.Contains(query, StringComparison.OrdinalIgnoreCase)) &&
                    p.IsActive && !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Product>> GetSellerProductsAsync(Guid sellerId)
  {
    var allProducts = await _productRepository.GetAllAsync();
    return allProducts
        .Where(p => p.SellerId == sellerId && p.IsActive && !p.IsDeleted)
        .ToList();
  }

  public async Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
  {
    var allProducts = await _productRepository.GetAllAsync();
    return allProducts
        .Where(p => p.Price >= minPrice && p.Price <= maxPrice && p.IsActive && !p.IsDeleted)
        .ToList();
  }
}