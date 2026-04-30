namespace floofy.Views;

public partial class FloofyPlus : ContentPage
{
  public FloofyPlus()
  {
    InitializeComponent();
  }

  private async void OnCallEmergencyClicked(object sender, EventArgs e)
  {
    try
    {
      // In a real app, this would initiate a phone call
      if (await DisplayAlertAsync("Emergency Hotline",
          "Calling emergency veterinary support at +1-800-FLOOFY-VET...",
          "Call", "Cancel"))
      {
        // Attempt to dial the emergency number
        try
        {
          if (DeviceInfo.Platform == DevicePlatform.Android ||
              DeviceInfo.Platform == DevicePlatform.iOS)
          {
            PhoneDialer.Open("+18008356839"); // 1-800-FLOOFY-VET
          }
          else
          {
            await DisplayAlertAsync("Emergency Support",
                "Please call +1-800-FLOOFY-VET for emergency veterinary support.",
                "OK");
          }
        }
        catch (ArgumentNullException)
        {
          await DisplayAlertAsync("Error", "Unable to initiate call. Please call manually.", "OK");
        }
        catch (Exception ex)
        {
          await DisplayAlertAsync("Error", $"Error: {ex.Message}", "OK");
        }
      }
    }
    catch (Exception ex)
    {
      await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
    }
  }

  private async void OnBookAppointmentClicked(object sender, EventArgs e)
  {
    try
    {
      var dialog = new BookAppointmentDialog();
      await Navigation.PushAsync(dialog);
    }
    catch (Exception ex)
    {
      await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
    }
  }

  private async void OnViewMedicalRecordsClicked(object sender, EventArgs e)
  {
    try
    {
      await DisplayAlertAsync("Medical Records",
          "Your medical records will be displayed here. This feature is coming soon.",
          "OK");
    }
    catch (Exception ex)
    {
      await DisplayAlertAsync("Error", $"An error occurred: {ex.Message}", "OK");
    }
  }
}