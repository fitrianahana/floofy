namespace floofy.Services;

using floofy.Data;
using floofy.Models;
using floofy.Models.Enums;

public class OrderService : IOrderService
{
  private readonly IRepository<Order> _orderRepository;
  private readonly IRepository<OrderLineItem> _orderLineItemRepository;
  private readonly IRepository<OrderHistory> _orderHistoryRepository;
  private readonly IRepository<Product> _productRepository;

  public OrderService(
      IRepository<Order> orderRepository,
      IRepository<OrderLineItem> orderLineItemRepository,
      IRepository<OrderHistory> orderHistoryRepository,
      IRepository<Product> productRepository)
  {
    _orderRepository = orderRepository;
    _orderLineItemRepository = orderLineItemRepository;
    _orderHistoryRepository = orderHistoryRepository;
    _productRepository = productRepository;
  }

  public async Task<Order> GetOrderByIdAsync(Guid orderId)
  {
    return (await _orderRepository.GetByIdAsync(orderId))!;
  }

  public async Task<List<Order>> GetUserOrdersAsync(Guid userId)
  {
    var allOrders = await _orderRepository.GetAllAsync();
    return allOrders.Where(o => o.BuyerId == userId && !o.IsDeleted).ToList();
  }

  public async Task<Order> CreateOrderAsync(Guid buyerId, List<CartItem> items, Guid shippingMethodId)
  {
    // Calculate totals from cart items
    decimal totalPrice = 0m;
    decimal totalDiscount = 0m;

    // Create new order
    var order = new Order
    {
      OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
      OrderDate = DateTime.UtcNow,
      Status = OrderStatus.Pending,
      PaymentStatus = PaymentStatus.Pending,
      ShippingStatus = ShippingStatus.NotShipped,
      BuyerId = buyerId,
      ShippingMethodId = shippingMethodId,
      Notes = "Order created from cart"
    };

    // Calculate totals from items
    foreach (var item in items)
    {
      var product = await _productRepository.GetByIdAsync(item.ProductId);
      if (product != null)
      {
        totalPrice += item.PriceAtAddTime * item.Quantity;
        totalDiscount += product.Discount * item.Quantity;
      }
    }

    order.TotalPrice = totalPrice;
    order.TotalDiscount = totalDiscount;
    order.TotalTax = totalPrice * 0.08m; // 8% tax
    order.FinalPrice = totalPrice - totalDiscount + order.TotalTax;

    // Insert order
    await _orderRepository.InsertAsync(order);

    // Create order line items
    foreach (var item in items)
    {
      var product = (await _productRepository.GetByIdAsync(item.ProductId))!;
      var lineItem = new OrderLineItem
      {
        OrderId = order.Id,
        ProductId = item.ProductId,
        Quantity = item.Quantity,
        UnitPrice = item.PriceAtAddTime,
        TotalPrice = item.PriceAtAddTime * item.Quantity,
        Discount = product.Discount
      };
      await _orderLineItemRepository.InsertAsync(lineItem);
    }

    // Create initial order history entry
    var orderHistory = new OrderHistory
    {
      OrderId = order.Id,
      PreviousStatus = OrderStatus.Pending,
      NewStatus = OrderStatus.Pending,
      Timestamp = DateTime.UtcNow,
      Notes = "Order created"
    };
    await _orderHistoryRepository.InsertAsync(orderHistory);
    return order;
  }

  public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
  {
    var order = (await _orderRepository.GetByIdAsync(orderId))!;
    var previousStatus = order.Status;
    order.Status = newStatus;
    order.MarkAsUpdated();
    await _orderRepository.UpdateAsync(order);

    var orderHistory = new OrderHistory
    {
      OrderId = orderId,
      PreviousStatus = previousStatus,
      NewStatus = newStatus,
      Timestamp = DateTime.UtcNow,
      Notes = $"Order status changed to {newStatus}"
    };
    await _orderHistoryRepository.InsertAsync(orderHistory);
  }
}