namespace floofy.Views;

public partial class RehomingPolicy : ContentPage
{
  public RehomingPolicy()
  {
    InitializeComponent();
  }

  private async void OnBackClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }

  private async void OnUnderstandClicked(object? sender, EventArgs e)
  {
    await Shell.Current.GoToAsync("..");
  }
}
