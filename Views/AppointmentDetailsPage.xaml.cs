using Hospital.Mobile.Models;
using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

[QueryProperty(nameof(AppointmentId), "id")]
public partial class AppointmentDetailsPage : ContentPage
{
    private readonly HospitalMobileDataService _data;
    private MobileAppointment? _appointment;

    public string AppointmentId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                LoadAppointment(id);
            }
        }
    }

    public AppointmentDetailsPage()
    {
        InitializeComponent();
        _data = HospitalMobileDataService.Instance;
    }

    private void LoadAppointment(int id)
    {
        _appointment = _data.GetAppointment(id);

        if (_appointment is null)
        {
            return;
        }

        TypeLabel.Text = _appointment.Type;
        TitleLabel.Text = _appointment.Title;
        PatientLabel.Text = _appointment.PatientName;
        DateLabel.Text = _appointment.StartTime.ToString("dd-MM-yyyy HH:mm");
        LocationLabel.Text = _appointment.Location;
        ReportLabel.Text = _appointment.MedicalReport;
    }

    private async void OnCompleteClicked(object? sender, EventArgs e)
    {
        if (_appointment is null)
        {
            return;
        }

        await DisplayAlert(
            "Afspraak afronden",
            "Deze knop is alvast voorbereid. De echte status-update bouwen we in de volgende stap.",
            "OK");
    }
}
