using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace floofy.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
  private bool _isLoading;
  private string _errorMessage = string.Empty;

  public bool IsLoading
  {
    get => _isLoading;
    set => SetProperty(ref _isLoading, value);
  }

  public string ErrorMessage
  {
    get => _errorMessage;
    set => SetProperty(ref _errorMessage, value);
  }

  public event PropertyChangedEventHandler? PropertyChanged;

  protected void SetProperty<T>(
    ref T backingField,
    T value,
    [CallerMemberName] string propertyName = "")
  {
    if (!Equals(backingField, value))
    {
      backingField = value;
      OnPropertyChanged(propertyName);
    }
  }

  protected void OnPropertyChanged([CallerMemberName] string propertyName = "") =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}