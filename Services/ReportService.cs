namespace floofy.Services;

using floofy.Data;
using floofy.Models;

public class ReportService : IReportService
{
  private readonly IRepository<SalesReport> _reportRepository;
  private readonly IRepository<Order> _orderRepository;

  public ReportService(
      IRepository<SalesReport> reportRepository,
      IRepository<Order> orderRepository)
  {
    _reportRepository = reportRepository;
    _orderRepository = orderRepository;
  }

  public async Task<SalesReport> GetSellerSalesReportAsync(Guid sellerId, DateTime startDate, DateTime endDate)
  {
    var allReports = await _reportRepository.GetAllAsync();
    var report = allReports
        .FirstOrDefault(r => r.SellerId == sellerId &&
                            r.ReportDate >= startDate &&
                            r.ReportDate <= endDate &&
                            !r.IsDeleted);
    return report ?? new SalesReport { SellerId = sellerId };
  }

  public async Task<decimal> GetSellerTotalSalesAsync(Guid sellerId)
  {
    var allOrders = await _orderRepository.GetAllAsync();
    var sellerOrders = allOrders
        .Where(o => !o.IsDeleted)
        .ToList();
    return sellerOrders.Sum(o => o.FinalPrice);
  }

  public async Task<int> GetSellerTotalOrdersAsync(Guid sellerId)
  {
    var allOrders = await _orderRepository.GetAllAsync();
    var sellerOrders = allOrders
        .Where(o => !o.IsDeleted)
        .ToList();
    return sellerOrders.Count;
  }

  public async Task<decimal> GetAverageOrderValueAsync(Guid sellerId)
  {
    var totalSales = await GetSellerTotalSalesAsync(sellerId);
    var totalOrders = await GetSellerTotalOrdersAsync(sellerId);
    if (totalOrders == 0)
      return 0m;
    return totalSales / totalOrders;
  }

  public async Task<List<SalesReport>> GetAllSalesReportsAsync()
  {
    var allReports = await _reportRepository.GetAllAsync();
    return allReports.Where(r => !r.IsDeleted).ToList();
  }
}