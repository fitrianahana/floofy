namespace floofy.Services;

using floofy.Models;

public interface IReportService
{
  Task<SalesReport> GetSellerSalesReportAsync(Guid sellerId, DateTime startDate, DateTime endDate);
  Task<decimal> GetSellerTotalSalesAsync(Guid sellerId);
  Task<int> GetSellerTotalOrdersAsync(Guid sellerId);
  Task<decimal> GetAverageOrderValueAsync(Guid sellerId);
  Task<List<SalesReport>> GetAllSalesReportsAsync();
}