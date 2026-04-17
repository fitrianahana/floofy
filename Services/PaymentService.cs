namespace floofy.Services;

using floofy.Data;
using floofy.Models;
using floofy.Models.Enums;

public class PaymentService : IPaymentService
{
  private readonly IRepository<Order> _orderRepository;

  public PaymentService(IRepository<Order> orderRepository)
  {
    _orderRepository = orderRepository;
  }

  public async Task<Order> ProcessPaymentAsync(Guid orderId, PaymentMethod paymentMethod)
  {
    var order = (await _orderRepository.GetByIdAsync(orderId))!;
    order.PaymentStatus = PaymentStatus.Completed;
    order.PaymentMethodId = paymentMethod.Id;
    order.MarkAsUpdated();
    await _orderRepository.UpdateAsync(order);
    return order;
  }

  public async Task<bool> RefundPaymentAsync(Guid orderId)
  {
    var order = (await _orderRepository.GetByIdAsync(orderId))!;
    order.PaymentStatus = PaymentStatus.Refunded;
    order.MarkAsUpdated();
    await _orderRepository.UpdateAsync(order);
    return true;
  }

  public async Task UpdatePaymentStatusAsync(Guid orderId, PaymentStatus status)
  {
    var order = (await _orderRepository.GetByIdAsync(orderId))!;
    order.PaymentStatus = status;
    order.MarkAsUpdated();
    await _orderRepository.UpdateAsync(order);
  }

  public async Task<List<Order>> GetOrdersByPaymentStatusAsync(PaymentStatus status)
  {
    var allOrders = await _orderRepository.GetAllAsync();
    return allOrders
        .Where(o => o.PaymentStatus == status && !o.IsDeleted)
        .ToList();
  }

  public async Task<bool> ValidatePaymentAsync(PaymentMethod paymentMethod)
  {
    // Basic validation: check if payment method is not null and not expired
    if (paymentMethod == null)
      return false;
    if (paymentMethod.IsExpired())
      return false;
    if (string.IsNullOrEmpty(paymentMethod.Type) ||
        string.IsNullOrEmpty(paymentMethod.LastFourDigits))
      return false;
    return true;
  }
}