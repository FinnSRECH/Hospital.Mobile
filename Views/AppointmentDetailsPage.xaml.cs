using Hospital.Mobile.Models;
using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

[QueryProperty(nameof(AppointmentId), "id")]
public partial class AppointmentDetailsPage : ContentPage
{
	private readonly HospitalMobileDataService _data;

	private MobileAppointment? _appointment;
	private MobilePatient? _patient;

	public string AppointmentId
	{
		set
		{
			if (int.TryParse(
					value,
					out var id))
			{
				_ = LoadAppointmentAsync(id);
			}
		}
	}

	public AppointmentDetailsPage()
	{
		InitializeComponent();

		_data =
			HospitalMobileDataService.Instance;
	}

	// -------------------------
	// GEGEVENS LADEN
	// -------------------------

	private async Task LoadAppointmentAsync(
		int id)
	{
		_appointment =
			await _data.GetAppointmentAsync(id);

		if (_appointment is null)
		{
			return;
		}

		_patient =
			await _data.GetPatientAsync(
				_appointment.PatientId);

		TypeLabel.Text =
			_appointment.Type;

		TitleLabel.Text =
			_appointment.Title;

		PatientLabel.Text =
			_appointment.PatientName;

		DateLabel.Text =
			_appointment.StartTime.ToString(
				"dd-MM-yyyy HH:mm");

		LocationLabel.Text =
			_appointment.Location;

		StatusLabel.Text =
			_appointment.Status;

		ReportLabel.Text =
			_appointment.MedicalReport;

		NoteEditor.Text =
			_appointment.Notes;

		// -------------------------
		// PATIENTGEGEVENS
		// -------------------------

		if (_patient is not null)
		{
			BirthDateLabel.Text =
				_patient.DateOfBirth.ToString(
					"dd-MM-yyyy");

			EmailLabel.Text =
				_patient.Email;

			PhoneLabel.Text =
				_patient.PhoneNumber;

			AddressLabel.Text =
				_patient.Address;

			TreatmentLabel.Text =
				_patient.ActiveTreatment;

			TreatmentStatusPicker.SelectedIndex =
				_patient.TreatmentStatus switch
				{
					"Active" => 0,
					"Completed" => 1,
					"Cancelled" => 2,
					_ => -1
				};
		}
		else
		{
			BirthDateLabel.Text =
				"-";

			EmailLabel.Text =
				"-";

			PhoneLabel.Text =
				"-";

			AddressLabel.Text =
				"-";

			TreatmentLabel.Text =
				"-";

			TreatmentStatusPicker.SelectedIndex =
				-1;
		}

		UpdateCompletedState();
	}

	// -------------------------
	// NOTITIE OPSLAAN
	// -------------------------

	private async void OnSaveNoteClicked(
		object? sender,
		EventArgs e)
	{
		if (_appointment is null)
		{
			return;
		}

		if (string.IsNullOrWhiteSpace(
				NoteEditor.Text))
		{
			await DisplayAlertAsync(
				"Notitie ontbreekt",
				"Vul eerst een notitie in.",
				"OK");

			return;
		}

		var saved =
			await _data.SaveNoteAsync(
				_appointment.Id,
				NoteEditor.Text);

		if (!saved)
		{
			await DisplayAlertAsync(
				"Opslaan mislukt",
				"De notitie kon niet worden opgeslagen.",
				"OK");

			return;
		}

		await DisplayAlertAsync(
			"Notitie opgeslagen",
			"De notitie is lokaal opgeslagen en blijft offline beschikbaar.",
			"OK");
	}

	// -------------------------
	// BEHANDELSTATUS
	// -------------------------

	private async void OnTreatmentStatusClicked(
		object? sender,
		EventArgs e)
	{
		if (_patient is null)
		{
			return;
		}

		if (TreatmentStatusPicker.SelectedItem
			is not string selectedStatus)
		{
			await DisplayAlertAsync(
				"Status ontbreekt",
				"Selecteer eerst een behandelstatus.",
				"OK");

			return;
		}

		var updated =
			await _data.UpdateTreatmentStatusAsync(
				_patient.Id,
				selectedStatus);

		if (!updated)
		{
			await DisplayAlertAsync(
				"Wijzigen mislukt",
				"De behandelstatus kon niet worden gewijzigd.",
				"OK");

			return;
		}

		await DisplayAlertAsync(
			"Status gewijzigd",
			"De behandelstatus is lokaal opgeslagen.",
			"OK");
	}

	// -------------------------
	// AFSPRAAK AFRONDEN
	// -------------------------

	private async void OnCompleteClicked(
		object? sender,
		EventArgs e)
	{
		if (_appointment is null)
		{
			return;
		}

		if (_appointment.IsCompleted)
		{
			await DisplayAlertAsync(
				"Afspraak afgerond",
				"Deze afspraak is al afgerond.",
				"OK");

			return;
		}

		var confirm =
			await DisplayAlertAsync(
				"Afspraak afronden",
				$"Weet je zeker dat je deze {_appointment.Type.ToLower()} wilt afronden?",
				"Afronden",
				"Annuleren");

		if (!confirm)
		{
			return;
		}

		var completed =
			await _data.CompleteAppointmentAsync(
				_appointment.Id);

		if (!completed)
		{
			await DisplayAlertAsync(
				"Afronden mislukt",
				"De afspraak kon niet worden afgerond.",
				"OK");

			return;
		}

		StatusLabel.Text =
			_appointment.Status;

		UpdateCompletedState();

		await DisplayAlertAsync(
			"Afspraak afgerond",
			"De afspraak is afgerond en lokaal opgeslagen.",
			"OK");
	}

	// -------------------------
	// UI STATUS
	// -------------------------

	private void UpdateCompletedState()
	{
		if (_appointment is null)
		{
			return;
		}

		if (_appointment.IsCompleted)
		{
			CompleteButton.Text =
				"Afspraak afgerond";

			CompleteButton.IsEnabled =
				false;
		}
		else
		{
			CompleteButton.Text =
				"Afspraak afronden";

			CompleteButton.IsEnabled =
				true;
		}
	}
}