using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Models.Enums;
using floofy.Services;
using System.Windows.Input;

namespace floofy.ViewModels;

public class BookingViewModel : BaseViewModel
{
  private readonly IBookingService _bookingService;
  private readonly SessionService _sessionService;
  private ObservableCollection<ServiceBooking> _bookings = new();
  private ServiceBooking? _selectedBooking;
  private DateTime _selectedDate = DateTime.UtcNow;
  private Guid _selectedServicePackageId = Guid.Empty;
  
  public ObservableCollection<ServiceBooking> Bookings
  {
    get => _bookings;
    set => SetProperty(ref _bookings, value);
  }
  
  public ServiceBooking? SelectedBooking
  {
    get => _selectedBooking;
    set => SetProperty(ref _selectedBooking, value);
  }
  
  public DateTime SelectedDate
  {
    get => _selectedDate;
    set => SetProperty(ref _selectedDate, value);
  }
  
  public Guid SelectedServicePackageId
  {
    get => _selectedServicePackageId;
    set => SetProperty(ref _selectedServicePackageId, value);
  }
  
  public ICommand LoadBookingsCommand { get; }
  public ICommand BookServiceCommand { get; }
  public ICommand CancelBookingCommand { get; }
  
  public BookingViewModel()
  {
    _bookingService = App.Services.GetRequiredService<IBookingService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    LoadBookingsCommand = new RelayCommand(async () => await OnLoadBookingsAsync());
    BookServiceCommand = new RelayCommand(async () => await OnBookServiceAsync());
    CancelBookingCommand = new RelayCommand<Guid>(async (bookingId) => await OnCancelBookingAsync(bookingId));
  }
  
  private async Task OnLoadBookingsAsync()
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        return;
      }
      var bookings = await _bookingService.GetUserBookingsAsync(userId.Value);
      Bookings = new ObservableCollection<ServiceBooking>(bookings);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load bookings: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }
  
  private async Task OnBookServiceAsync()
  {
    if (SelectedServicePackageId == Guid.Empty)
    {
      ErrorMessage = "Please select a service package";
      return;
    }
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      var userId = _sessionService.CurrentUser?.Id;
      if (userId == null)
      {
        ErrorMessage = "User not logged in";
        return;
      }
      await _bookingService.CreateBookingAsync(userId.Value, SelectedServicePackageId, SelectedDate);
      await OnLoadBookingsAsync();
      ErrorMessage = "Booking created successfully!";
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to book service: {ex.Message}";
      IsLoading = false;
    }
  }
  
  private async Task OnCancelBookingAsync(Guid bookingId)
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      await _bookingService.CancelBookingAsync(bookingId);
      await OnLoadBookingsAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to cancel booking: {ex.Message}";
      IsLoading = false;
    }
  }
}