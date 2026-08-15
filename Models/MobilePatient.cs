namespace Hospital.Mobile.Models;

public class MobilePatient
{
	public int Id { get; set; }

	public string FullName { get; set; } =
		string.Empty;

	public DateTime DateOfBirth { get; set; }

	public string Email { get; set; } =
		string.Empty;

	public string PhoneNumber { get; set; } =
		string.Empty;

	public string Address { get; set; } =
		string.Empty;

	public string ActiveTreatment { get; set; } =
		string.Empty;

	public string TreatmentStatus { get; set; } =
		string.Empty;
}