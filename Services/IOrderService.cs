using floofy.Models;
using floofy.Models.Enums;

namespace floofy.Services;

public interface IOrderService
{
  Task<Order> GetOrderByIdAsync(Guid orderId);
  Task<List<Order>> GetUserOrdersAsync(Guid userId);
  Task<Order> CreateOrderAsync(Guid buyerId, List<CartItem> items, Guid shippingMethodId);
  Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
}