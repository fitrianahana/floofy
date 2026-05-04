namespace floofy.Views;

public partial class RehomingAgreement : ContentPage
{
  public RehomingAgreement()
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
