using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

public partial class PlanningPage : ContentPage
{
	private readonly HospitalMobileDataService _data;

	public PlanningPage()
	{
		InitializeComponent();

		_data =
			HospitalMobileDataService.Instance;
	}

	// -------------------------
	// PLANNING LADEN
	// -------------------------

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		await LoadPlanningAsync();
	}

	private async Task LoadPlanningAsync()
	{
		var planning =
			await _data.GetPersonalPlanningAsync();

		PlanningList.ItemsSource =
			planning;
	}

	// -------------------------
	// AFSPRAAK OPENEN
	// -------------------------

	private async void OnViewClicked(
		object? sender,
		EventArgs e)
	{
		if (sender is not Button button ||
			!int.TryParse(
				button.CommandParameter?.ToString(),
				out var id))
		{
			return;
		}

		await Shell.Current.GoToAsync(
			$"{nameof(AppointmentDetailsPage)}?id={id}");
	}
}