using Hospital.Mobile.Models;
using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

public partial class HomePage : ContentPage
{
	private readonly HospitalMobileDataService _data;

	private MobileAppointment? _nextAppointment;

	public HomePage()
	{
		InitializeComponent();

		_data =
			HospitalMobileDataService.Instance;
	}

	// -------------------------
	// DASHBOARD LADEN
	// -------------------------

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await LoadDashboardAsync();
	}

	private async Task LoadDashboardAsync()
	{
		var planning =
			await _data.GetPersonalPlanningAsync();

		var todayAppointments =
			planning
				.Where(a =>
					a.StartTime.Date ==
					DateTime.Today)
				.ToList();

		TodayCountLabel.Text =
			todayAppointments.Count
				.ToString();

		CompletedCountLabel.Text =
			todayAppointments
				.Count(a =>
					a.IsCompleted)
				.ToString();

		_nextAppointment =
			planning
				.Where(a =>
					!a.IsCompleted &&
					a.StartTime >= DateTime.Now)
				.OrderBy(a =>
					a.StartTime)
				.FirstOrDefault();

		if (_nextAppointment is null)
		{
			NextAppointmentTypeLabel.Text =
				"Geen volgende afspraak";

			NextAppointmentTitleLabel.Text =
				"Er staat momenteel geen volgende afspraak gepland.";

			NextAppointmentPatientLabel.IsVisible =
				false;

			NextAppointmentInfoLabel.IsVisible =
				false;

			NextAppointmentButton.IsVisible =
				false;

			return;
		}

		NextAppointmentTypeLabel.Text =
			_nextAppointment.Type;

		NextAppointmentTitleLabel.Text =
			_nextAppointment.Title;

		NextAppointmentPatientLabel.Text =
			_nextAppointment.PatientName;

		NextAppointmentPatientLabel.IsVisible =
			true;

		NextAppointmentInfoLabel.Text =
			$"{_nextAppointment.StartTime:dd-MM-yyyy HH:mm} · {_nextAppointment.Location}";

		NextAppointmentInfoLabel.IsVisible =
			true;

		NextAppointmentButton.IsVisible =
			true;
	}

	// -------------------------
	// VOLGENDE AFSPRAAK
	// -------------------------

	private async void OnNextAppointmentClicked(
		object? sender,
		EventArgs e)
	{
		if (_nextAppointment is null)
		{
			return;
		}

		await Shell.Current.GoToAsync(
			$"{nameof(AppointmentDetailsPage)}?id={_nextAppointment.Id}");
	}

	// -------------------------
	// PLANNING
	// -------------------------

	private async void OnPlanningClicked(
		object? sender,
		EventArgs e)
	{
		await Shell.Current.GoToAsync(
			nameof(PlanningPage));
	}
}