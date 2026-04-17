namespace floofy.Services;

using floofy.Models;
using floofy.Models.Enums;

public interface IPaymentService
{
  Task<Order> ProcessPaymentAsync(Guid orderId, PaymentMethod paymentMethod);
  Task<bool> RefundPaymentAsync(Guid orderId);
  Task UpdatePaymentStatusAsync(Guid orderId, PaymentStatus status);
  Task<List<Order>> GetOrdersByPaymentStatusAsync(PaymentStatus status);
  Task<bool> ValidatePaymentAsync(PaymentMethod paymentMethod);
}