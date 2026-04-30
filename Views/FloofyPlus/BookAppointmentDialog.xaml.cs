namespace floofy.Views;

public partial class BookAppointmentDialog : ContentPage
{
  private string? _selectedService;

  public BookAppointmentDialog()
  {
    InitializeComponent();
    
    // Add tap gestures to service buttons
    var vetConsultationTapGesture = new TapGestureRecognizer();
    vetConsultationTapGesture.Tapped += (s, e) => SelectService("Vet Consultation", VetConsultationButton);
    VetConsultationButton.GestureRecognizers.Add(vetConsultationTapGesture);

    var vaccinationTapGesture = new TapGestureRecognizer();
    vaccinationTapGesture.Tapped += (s, e) => SelectService("Vaccination", VaccinationButton);
    VaccinationButton.GestureRecognizers.Add(vaccinationTapGesture);

    var healthCheckupTapGesture = new TapGestureRecognizer();
    healthCheckupTapGesture.Tapped += (s, e) => SelectService("Health Checkup", HealthCheckupButton);
    HealthCheckupButton.GestureRecognizers.Add(healthCheckupTapGesture);

    var surgeryTapGesture = new TapGestureRecognizer();
    surgeryTapGesture.Tapped += (s, e) => SelectService("Surgery & Treatment", SurgeryButton);
    SurgeryButton.GestureRecognizers.Add(surgeryTapGesture);
  }

  private void SelectService(string serviceName, Border selectedBorder)
  {
    _selectedService = serviceName;
    
    // Reset all borders to default style
    ResetServiceBorder(VetConsultationButton);
    ResetServiceBorder(VaccinationButton);
    ResetServiceBorder(HealthCheckupButton);
    ResetServiceBorder(SurgeryButton);

    // Highlight selected border
    selectedBorder.Stroke = Color.FromArgb("#B19CD9");
    selectedBorder.StrokeThickness = 2;
    selectedBorder.BackgroundColor = Color.FromArgb("#F0E8F8");

    // Enable continue button
    ContinueButton.IsEnabled = true;
  }

  private void ResetServiceBorder(Border border)
  {
    border.Stroke = Color.FromArgb("#DDD2FF");
    border.StrokeThickness = 1;
    border.BackgroundColor = Color.FromArgb("#F8F7FC");
  }

  private async void OnCancelClicked(object sender, EventArgs e)
  {
    await Navigation.PopAsync();
  }

  private async void OnContinueClicked(object sender, EventArgs e)
  {
    await DisplayAlertAsync("Appointment Request",
        $"You've selected: {_selectedService}\n\nOur team will contact you shortly to confirm your appointment.",
        "OK");
    
    await Navigation.PopAsync();
  }
}
