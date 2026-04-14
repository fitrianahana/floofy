
using floofy.Models.Enums;

namespace floofy.Models;

public class SalesReport : Entity
{
  public Guid SellerId { get; set; }
  public decimal TotalSales { get; set; }
  public int TotalOrders { get; set; }
  public decimal TotalRevenue { get; set; }
  public DateTime ReportDate { get; set; }
  public string Period { get; set; } = string.Empty;

  public decimal CalculateAverageOrderValue()
  {
    return TotalOrders > 0 ? TotalRevenue / TotalOrders : 0;
  }

  public void CalculateMetrics(List<Order> sellerOrders)
  {
    TotalOrders = sellerOrders.Count;
    TotalRevenue = sellerOrders.Sum(o => o.FinalPrice);
    TotalSales = sellerOrders.Where(o => o.Status == OrderStatus.Delivered).Count();
    ReportDate = DateTime.UtcNow;
  }
}