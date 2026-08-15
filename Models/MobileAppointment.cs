namespace Hospital.Mobile.Models;

public class MobileAppointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";
    public string MedicalReport { get; set; } = string.Empty;
}
