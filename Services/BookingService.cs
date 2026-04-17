namespace floofy.Services;

using floofy.Data;
using floofy.Models;
using floofy.Models.Enums;

public class BookingService : IBookingService
{
  private readonly IRepository<ServiceBooking> _bookingRepository;
  private readonly IRepository<ServicePackage> _servicePackageRepository;

  public BookingService(
      IRepository<ServiceBooking> bookingRepository,
      IRepository<ServicePackage> servicePackageRepository)
  {
    _bookingRepository = bookingRepository;
    _servicePackageRepository = servicePackageRepository;
  }

  public async Task<ServiceBooking> GetBookingByIdAsync(Guid bookingId)
  {
    return (await _bookingRepository.GetByIdAsync(bookingId))!;
  }

  public async Task<List<ServiceBooking>> GetUserBookingsAsync(Guid userId)
  {
    var allBookings = await _bookingRepository.GetAllAsync();
    return allBookings.Where(b => b.BuyerId == userId && !b.IsDeleted).ToList();
  }

  public async Task<List<ServiceBooking>> GetSellerBookingsAsync(Guid sellerId)
  {
    var allBookings = await _bookingRepository.GetAllAsync();
    var allServicePackages = await _servicePackageRepository.GetAllAsync();
    return allBookings
        .Where(b => allServicePackages.Any(sp => sp.Id == b.ServicePackageId && sp.SellerId == sellerId) && !b.IsDeleted)
        .ToList();
  }

  public async Task<ServiceBooking> CreateBookingAsync(Guid userId, Guid servicePackageId, DateTime bookingDate)
  {
    var booking = new ServiceBooking
    {
      BuyerId = userId,
      ServicePackageId = servicePackageId,
      BookingDate = bookingDate,
      Status = BookingStatus.Pending
    };
    await _bookingRepository.InsertAsync(booking);
    return booking;
  }

  public async Task UpdateBookingStatusAsync(Guid bookingId, BookingStatus newStatus)
  {
    var booking = (await _bookingRepository.GetByIdAsync(bookingId))!;
    booking.Status = newStatus;
    booking.MarkAsUpdated();
    await _bookingRepository.UpdateAsync(booking);
  }

  public async Task CancelBookingAsync(Guid bookingId)
  {
    var booking = (await _bookingRepository.GetByIdAsync(bookingId))!;
    booking.Status = BookingStatus.Cancelled;
    booking.MarkAsUpdated();
    await _bookingRepository.UpdateAsync(booking);
  }
}