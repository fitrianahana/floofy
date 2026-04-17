namespace floofy.Services;

using floofy.Models;
using floofy.Models.Enums;

public interface IBookingService
{
  Task<ServiceBooking> GetBookingByIdAsync(Guid bookingId);
  Task<List<ServiceBooking>> GetUserBookingsAsync(Guid userId);
  Task<List<ServiceBooking>> GetSellerBookingsAsync(Guid sellerId);
  Task<ServiceBooking> CreateBookingAsync(Guid userId, Guid servicePackageId, DateTime bookingDate);
  Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus);
  Task CancelBookingAsync(Guid bookingId);
}