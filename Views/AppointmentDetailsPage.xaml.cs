using Hospital.Mobile.Models;
using Hospital.Mobile.Services;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

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

		LoadExistingPhoto();

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
	// FOTO MAKEN
	// -------------------------

	private async void OnTakePhotoClicked(
		object? sender,
		EventArgs e)
	{
		if (_appointment is null)
		{
			return;
		}

		if (!MediaPicker.Default.IsCaptureSupported)
		{
			await DisplayAlertAsync(
				"Camera niet beschikbaar",
				"Op dit apparaat kan de camera niet vanuit de app worden gebruikt.",
				"OK");

			return;
		}

		try
		{
			var photo =
				await MediaPicker.Default
					.CapturePhotoAsync();

			if (photo is null)
			{
				return;
			}

			await SavePhotoAsync(
				photo,
				"Foto toegevoegd",
				"De gemaakte foto is succesvol aan deze afspraak gekoppeld.");
		}
		catch (PermissionException)
		{
			await DisplayAlertAsync(
				"Geen cameratoegang",
				"Geef Hospital Mobile toestemming om de camera te gebruiken.",
				"OK");
		}
		catch (Exception)
		{
			await DisplayAlertAsync(
				"Foto maken mislukt",
				"De foto kon niet worden gemaakt of opgeslagen.",
				"OK");
		}
	}

	// -------------------------
	// FOTO SELECTEREN
	// -------------------------

	private async void OnPickPhotoClicked(
		object? sender,
		EventArgs e)
	{
		if (_appointment is null)
		{
			return;
		}

		try
		{
			var photo =
				await MediaPicker.Default
					.PickPhotoAsync();

			if (photo is null)
			{
				return;
			}

			await SavePhotoAsync(
				photo,
				"Foto geselecteerd",
				"De geselecteerde foto is succesvol aan deze afspraak gekoppeld.");
		}
		catch (PermissionException)
		{
			await DisplayAlertAsync(
				"Geen toegang tot foto's",
				"Geef Hospital Mobile toestemming om foto's te selecteren.",
				"OK");
		}
		catch (Exception)
		{
			await DisplayAlertAsync(
				"Foto selecteren mislukt",
				"De foto kon niet worden geselecteerd of opgeslagen.",
				"OK");
		}
	}

	// -------------------------
	// FOTO LOKAAL OPSLAAN
	// -------------------------

	private async Task SavePhotoAsync(
		FileResult photo,
		string successTitle,
		string successMessage)
	{
		if (_appointment is null)
		{
			return;
		}

		DeleteExistingPhotos(
			_appointment.Id);

		var extension =
			Path.GetExtension(
				photo.FileName);

		if (string.IsNullOrWhiteSpace(
				extension))
		{
			extension =
				".jpg";
		}

		var localFileName =
			$"appointment-{_appointment.Id}-{Guid.NewGuid()}{extension}";

		var localFilePath =
			Path.Combine(
				FileSystem.AppDataDirectory,
				localFileName);

		await using var sourceStream =
			await photo.OpenReadAsync();

		await using var localFileStream =
			File.Open(
				localFilePath,
				FileMode.Create,
				FileAccess.Write);

		await sourceStream.CopyToAsync(
			localFileStream);

		AppointmentPhoto.Source =
			ImageSource.FromFile(
				localFilePath);

		AppointmentPhoto.IsVisible =
			true;

		PhotoStatusLabel.Text =
			"Foto lokaal opgeslagen bij deze afspraak.";

		await DisplayAlertAsync(
			successTitle,
			successMessage,
			"OK");
	}

	// -------------------------
	// BESTAANDE FOTO LADEN
	// -------------------------

	private void LoadExistingPhoto()
	{
		if (_appointment is null)
		{
			return;
		}

		var photos =
			Directory.GetFiles(
				FileSystem.AppDataDirectory,
				$"appointment-{_appointment.Id}-*");

		var photoPath =
			photos.FirstOrDefault();

		if (photoPath is null)
		{
			AppointmentPhoto.IsVisible =
				false;

			PhotoStatusLabel.Text =
				"Nog geen foto toegevoegd.";

			return;
		}

		AppointmentPhoto.Source =
			ImageSource.FromFile(
				photoPath);

		AppointmentPhoto.IsVisible =
			true;

		PhotoStatusLabel.Text =
			"Foto lokaal opgeslagen bij deze afspraak.";
	}

	// -------------------------
	// OUDE FOTO VERWIJDEREN
	// -------------------------

	private static void DeleteExistingPhotos(
		int appointmentId)
	{
		var photos =
			Directory.GetFiles(
				FileSystem.AppDataDirectory,
				$"appointment-{appointmentId}-*");

		foreach (var photoPath in photos)
		{
			File.Delete(
				photoPath);
		}
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