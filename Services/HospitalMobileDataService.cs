using Hospital.Mobile.Models;

namespace Hospital.Mobile.Services;

public class HospitalMobileDataService
{
	public static HospitalMobileDataService Instance { get; } =
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

	public IReadOnlyList<MobileAppointment>
		GetPersonalPlanning()
	{
		return _appointments
			.OrderBy(a => a.StartTime)
			.ToList();
	}

	public MobileAppointment? GetAppointment(
		int id)
	{
		return _appointments
			.FirstOrDefault(a =>
				a.Id == id);
	}

	// -------------------------
	// PATIENTEN
	// -------------------------

	public MobilePatient? GetPatient(
		int patientId)
	{
		return _patients
			.FirstOrDefault(p =>
				p.Id == patientId);
	}

	// -------------------------
	// NOTITIES
	// -------------------------

	public bool SaveNote(
		int appointmentId,
		string note)
	{
		var appointment =
			GetAppointment(appointmentId);

		if (appointment is null)
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(note))
		{
			return false;
		}

		appointment.Notes =
			note.Trim();

		return true;
	}

	// -------------------------
	// AFSPRAAK AFRONDEN
	// -------------------------

	public bool CompleteAppointment(
		int appointmentId)
	{
		var appointment =
			GetAppointment(appointmentId);

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

		return true;
	}

	// -------------------------
	// BEHANDELSTATUS
	// -------------------------

	public bool UpdateTreatmentStatus(
		int patientId,
		string status)
	{
		var patient =
			GetPatient(patientId);

		if (patient is null)
		{
			return false;
		}

		if (string.IsNullOrWhiteSpace(status))
		{
			return false;
		}

		patient.TreatmentStatus =
			status.Trim();

		return true;
	}
}