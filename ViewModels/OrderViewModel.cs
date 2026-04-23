using System.Collections.ObjectModel;
using floofy.Models;
using floofy.Models.Enums;
using floofy.Services;
using System.Windows.Input;
namespace floofy.ViewModels;

public class OrderViewModel : BaseViewModel
{
  private readonly IOrderService _orderService;
  private readonly SessionService _sessionService;
  private ObservableCollection<Order> _orders = new();
  private Order? _selectedOrder;
  private OrderStatus _filterStatus = OrderStatus.Pending;

  public ObservableCollection<Order> Orders
  {
    get => _orders;
    set => SetProperty(ref _orders, value);
  }

  public Order? SelectedOrder
  {
    get => _selectedOrder;
    set => SetProperty(ref _selectedOrder, value);
  }

  public OrderStatus FilterStatus
  {
    get => _filterStatus;
    set => SetProperty(ref _filterStatus, value);
  }

  public ICommand LoadOrdersCommand { get; }
  public ICommand FilterOrdersCommand { get; }
  public ICommand CancelOrderCommand { get; }

  public OrderViewModel()
  {
    _orderService = App.Services.GetRequiredService<IOrderService>();
    _sessionService = App.Services.GetRequiredService<SessionService>();
    LoadOrdersCommand = new RelayCommand(async () => await OnLoadOrdersAsync());
    FilterOrdersCommand = new RelayCommand(async () => await OnFilterOrdersAsync());
    CancelOrderCommand = new RelayCommand<Guid>(async (orderId) => await OnCancelOrderAsync(orderId));
  }

  private async Task OnLoadOrdersAsync()
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
      var orders = await _orderService.GetUserOrdersAsync(userId.Value);
      Orders = new ObservableCollection<Order>(orders);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to load orders: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnFilterOrdersAsync()
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
      var orders = await _orderService.GetUserOrdersAsync(userId.Value);
      var filteredOrders = orders.Where(o => o.Status == FilterStatus).ToList();
      Orders = new ObservableCollection<Order>(filteredOrders);
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to filter orders: {ex.Message}";
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task OnCancelOrderAsync(Guid orderId)
  {
    ErrorMessage = string.Empty;
    IsLoading = true;
    try
    {
      await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
      await OnLoadOrdersAsync();
    }
    catch (Exception ex)
    {
      ErrorMessage = $"Failed to cancel order: {ex.Message}";
      IsLoading = false;
    }
  }
}