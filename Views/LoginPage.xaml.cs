using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

public partial class LoginPage : ContentPage
{
	private readonly HospitalMobileDataService _data;

	public LoginPage()
	{
		InitializeComponent();

		_data =
			HospitalMobileDataService.Instance;
	}

	// -------------------------
	// INLOGGEN
	// -------------------------

	private async void OnLoginClicked(
		object? sender,
		EventArgs e)
	{
		await LoginAsync();
	}

	private async Task LoginAsync()
	{
		var email =
			EmailEntry.Text?.Trim()
			?? string.Empty;

		var password =
			PasswordEntry.Text
			?? string.Empty;

		HideError();

		// -------------------------
		// VALIDATIE
		// -------------------------

		if (string.IsNullOrWhiteSpace(
				email))
		{
			ShowError(
				"Vul je e-mailadres in.");

			EmailEntry.Focus();

			return;
		}

		if (string.IsNullOrWhiteSpace(
				password))
		{
			ShowError(
				"Vul je wachtwoord in.");

			PasswordEntry.Focus();

			return;
		}

		// -------------------------
		// LOGIN CONTROLEREN
		// -------------------------

		if (!_data.Login(
				email,
				password))
		{
			ShowError(
				"E-mailadres of wachtwoord is onjuist.");

			return;
		}

		HideError();

		await Shell.Current.GoToAsync(
			nameof(HomePage));
	}

	// -------------------------
	// ENTER BIJ WACHTWOORD
	// -------------------------

	private async void OnPasswordCompleted(
		object? sender,
		EventArgs e)
	{
		await LoginAsync();
	}

	// -------------------------
	// WACHTWOORD VERGETEN
	// -------------------------

	private async void OnForgotPasswordClicked(
		object? sender,
		EventArgs e)
	{
		await DisplayAlertAsync(
			"Wachtwoord vergeten?",
			"Neem contact op met de beheerder van het ziekenhuis om je toegang te laten herstellen.",
			"OK");
	}

	// -------------------------
	// FOUTMELDING TONEN
	// -------------------------

	private void ShowError(
		string message)
	{
		ErrorLabel.Text =
			message;

		ErrorContainer.IsVisible =
			true;
	}

	// -------------------------
	// FOUTMELDING VERBERGEN
	// -------------------------

	private void HideError()
	{
		ErrorLabel.Text =
			string.Empty;

		ErrorContainer.IsVisible =
			false;
	}
}