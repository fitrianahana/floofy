namespace floofy.Views;

public partial class FloofyPlus : ContentPage
{
  public FloofyPlus()
  {
    InitializeComponent();
  }

  private async void OnCallEmergencyClicked(object sender, EventArgs e)
  {
    await DisplayAlertAsync("Emergency Hotline", "Calling emergency veterinary support...", "OK");
  }

  private async void OnBookAppointmentClicked(object sender, EventArgs e)
  {
    await DisplayAlertAsync("Book Appointment", "Appointment booking flow is coming soon.", "OK");
  }

  private async void OnViewMedicalRecordsClicked(object sender, EventArgs e)
  {
    await DisplayAlertAsync("Medical Records", "Medical records module is coming soon.", "OK");
  }
}