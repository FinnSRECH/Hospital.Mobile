using SQLite;

namespace Hospital.Mobile.Services;

public class MobileDatabaseService
{
	private SQLiteAsyncConnection? _database;

	private const string DatabaseFilename =
		"HospitalMobile.db3";

	private static string DatabasePath =>
		Path.Combine(
			FileSystem.AppDataDirectory,
			DatabaseFilename);

	// -------------------------
	// DATABASE INITIALISEREN
	// -------------------------

	private async Task InitAsync()
	{
		if (_database is not null)
		{
			return;
		}

		_database =
			new SQLiteAsyncConnection(
				DatabasePath);

		await _database.CreateTableAsync<AppointmentState>();

		await _database.CreateTableAsync<PatientTreatmentState>();
	}

	// -------------------------
	// AFSPRAAKGEGEVENS
	// -------------------------

	public async Task<AppointmentState?>
		GetAppointmentStateAsync(
			int appointmentId)
	{
		await InitAsync();

		return await _database!
			.Table<AppointmentState>()
			.Where(x =>
				x.AppointmentId == appointmentId)
			.FirstOrDefaultAsync();
	}

	public async Task SaveNoteAsync(
		int appointmentId,
		string note)
	{
		await InitAsync();

		var state =
			await GetOrCreateAppointmentStateAsync(
				appointmentId);

		state.Note =
			note.Trim();

		await _database!
			.InsertOrReplaceAsync(
				state);
	}

	public async Task SaveAppointmentStatusAsync(
		int appointmentId,
		string status)
	{
		await InitAsync();

		var state =
			await GetOrCreateAppointmentStateAsync(
				appointmentId);

		state.AppointmentStatus =
			status;

		await _database!
			.InsertOrReplaceAsync(
				state);
	}

	private async Task<AppointmentState>
		GetOrCreateAppointmentStateAsync(
			int appointmentId)
	{
		var existing =
			await _database!
				.Table<AppointmentState>()
				.Where(x =>
					x.AppointmentId == appointmentId)
				.FirstOrDefaultAsync();

		if (existing is not null)
		{
			return existing;
		}

		return new AppointmentState
		{
			AppointmentId =
				appointmentId
		};
	}

	// -------------------------
	// BEHANDELSTATUS
	// -------------------------

	public async Task<PatientTreatmentState?>
		GetTreatmentStateAsync(
			int patientId)
	{
		await InitAsync();

		return await _database!
			.Table<PatientTreatmentState>()
			.Where(x =>
				x.PatientId == patientId)
			.FirstOrDefaultAsync();
	}

	public async Task SaveTreatmentStatusAsync(
		int patientId,
		string status)
	{
		await InitAsync();

		var state =
			await GetTreatmentStateAsync(
				patientId);

		if (state is null)
		{
			state =
				new PatientTreatmentState
				{
					PatientId =
						patientId
				};
		}

		state.TreatmentStatus =
			status;

		await _database!
			.InsertOrReplaceAsync(
				state);
	}
}

// -------------------------
// SQLITE MODEL AFSPRAAK
// -------------------------

public class AppointmentState
{
	[PrimaryKey]
	public int AppointmentId { get; set; }

	public string Note { get; set; } =
		string.Empty;

	public string AppointmentStatus { get; set; } =
		"Planned";
}

// -------------------------
// SQLITE MODEL BEHANDELING
// -------------------------

public class PatientTreatmentState
{
	[PrimaryKey]
	public int PatientId { get; set; }

	public string TreatmentStatus { get; set; } =
		"Active";
}