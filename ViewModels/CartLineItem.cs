using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace floofy.ViewModels;

public class CartLineItem : INotifyPropertyChanged
{
  private int _quantity;

  public Guid CartItemId { get; init; }
  public Guid ProductId { get; init; }
  public string ProductName { get; init; } = string.Empty;
  public string Thumbnail { get; init; } = string.Empty;
  public decimal UnitPrice { get; init; }

  public int Quantity
  {
    get => _quantity;
    set
    {
      if (_quantity != value)
      {
        _quantity = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(Subtotal));
      }
    }
  }

  public decimal Subtotal => UnitPrice * Quantity;

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
