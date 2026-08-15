using Hospital.Mobile.Models;

namespace Hospital.Mobile.Services;

public class HospitalMobileDataService
{
	public static HospitalMobileDataService Instance { get; } =
		new();

	private readonly MobileDatabaseService _database =
		new();

	private readonly List<MobilePatient> _patients =
	[
		new MobilePatient
		{
			Id = 1,
			FullName = "Jan de Vries",
			DateOfBirth = new DateTime(1985, 4, 12),
			Email = "jan.devries@example.nl",
			PhoneNumber = "0612345678",
			Address = "Kerkstraat 12, Amsterdam",
			ActiveTreatment = "Behandeling knieklachten",
			TreatmentStatus = "Active"
		}
	];

	private readonly List<MobileAppointment> _appointments =
	[
		new MobileAppointment
		{
			Id = 1,
			PatientId = 1,
			PatientName = "Jan de Vries",
			Type = "Consultatie",
			Title = "Controle knie",
			StartTime = DateTime.Today
				.AddDays(1)
				.AddHours(9),
			Location = "B2.14",
			Status = "Planned",
			MedicalReport =
				"Controle van de aanhoudende knieklachten."
		},

		new MobileAppointment
		{
			Id = 2,
			PatientId = 1,
			PatientName = "Jan de Vries",
			Type = "Operatie",
			Title = "Kijkoperatie rechterknie",
			StartTime = DateTime.Today
				.AddDays(3)
				.AddHours(8),
			Location = "OK 1",
			Status = "Planned",
			MedicalReport =
				"Operatieve behandeling binnen de actieve knie-behandeling."
		}
	];

	// -------------------------
	// LOGIN
	// -------------------------

	public bool Login(
		string email,
		string password)
	{
		return email.Equals(
				   "emma.jansen@hospital.nl",
				   StringComparison.OrdinalIgnoreCase) &&
			   password == "Welkom123!";
	}

	// -------------------------
	// PLANNING
	// -------------------------

	public async Task<IReadOnlyList<MobileAppointment>>
		GetPersonalPlanningAsync()
	{
		foreach (var appointment in _appointments)
		{
			await LoadAppointmentStateAsync(
				appointment);
		}

		return _appointments
			.OrderBy(a =>
				a.StartTime)
			.ToList();
	}

	public async Task<MobileAppointment?>
		GetAppointmentAsync(
			int id)
	{
		var appointment =
			_appointments
				.FirstOrDefault(a =>
					a.Id == id);

		if (appointment is null)
		{
			return null;
		}

		await LoadAppointmentStateAsync(
			appointment);

		return appointment;
	}

	private async Task LoadAppointmentStateAsync(
		MobileAppointment appointment)
	{
		var state =
			await _database
				.GetAppointmentStateAsync(
					appointment.Id);

		if (state is null)
		{
			return;
		}

		appointment.Notes =
			state.Note;

		appointment.Status =
			state.AppointmentStatus;
	}

	// -------------------------
	// PATIENTEN
	// -------------------------

	public async Task<MobilePatient?>
		GetPatientAsync(
			int patientId)
	{
		var patient =
			_patients
				.FirstOrDefault(p =>
					p.Id == patientId);

		if (patient is null)
		{
			return null;
		}

		var state =
			await _database
				.GetTreatmentStateAsync(
					patientId);

		if (state is not null)
		{
			patient.TreatmentStatus =
				state.TreatmentStatus;
		}

		return patient;
	}

	// -------------------------
	// NOTITIES
	// -------------------------

	public async Task<bool> SaveNoteAsync(
		int appointmentId,
		string note)
	{
		var appointment =
			_appointments
				.FirstOrDefault(a =>
					a.Id == appointmentId);

		if (appointment is null)
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(
				note))
		{
			return false;
		}

		appointment.Notes =
			note.Trim();

		await _database
			.SaveNoteAsync(
				appointmentId,
				appointment.Notes);

		return true;
	}

	// -------------------------
	// AFSPRAAK AFRONDEN
	// -------------------------

	public async Task<bool>
		CompleteAppointmentAsync(
			int appointmentId)
	{
		var appointment =
			_appointments
				.FirstOrDefault(a =>
					a.Id == appointmentId);

		if (appointment is null)
		{
			return false;
		}

		if (appointment.IsCompleted)
		{
			return false;
		}

		appointment.Status =
			"Completed";

		await _database
			.SaveAppointmentStatusAsync(
				appointmentId,
				appointment.Status);

		return true;
	}

	// -------------------------
	// BEHANDELSTATUS
	// -------------------------

	public async Task<bool>
		UpdateTreatmentStatusAsync(
			int patientId,
			string status)
	{
		var patient =
			_patients
				.FirstOrDefault(p =>
					p.Id == patientId);

		if (patient is null)
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(
				status))
		{
			return false;
		}

		patient.TreatmentStatus =
			status.Trim();

		await _database
			.SaveTreatmentStatusAsync(
				patientId,
				patient.TreatmentStatus);

		return true;
	}
}