using Hospital.Mobile.Models;

namespace Hospital.Mobile.Services;

public class HospitalMobileDataService
{
    public static HospitalMobileDataService Instance { get; } = new();
    private readonly List<MobileAppointment> _appointments =
    [
        new MobileAppointment
        {
            Id = 1,
            PatientId = 1,
            PatientName = "Jan de Vries",
            Type = "Consultatie",
            Title = "Controle knie",
            StartTime = DateTime.Today.AddDays(1).AddHours(9),
            Location = "B2.14",
            Status = "Planned",
            MedicalReport = "Controle van de aanhoudende knieklachten."
        },
        new MobileAppointment
        {
            Id = 2,
            PatientId = 1,
            PatientName = "Jan de Vries",
            Type = "Operatie",
            Title = "Kijkoperatie rechterknie",
            StartTime = DateTime.Today.AddDays(3).AddHours(8),
            Location = "OK 1",
            Status = "Planned",
            MedicalReport = "Operatieve behandeling binnen de actieve knie-behandeling."
        }
    ];

    public bool Login(string email, string password)
    {
        return email.Equals(
                   "emma.jansen@hospital.nl",
                   StringComparison.OrdinalIgnoreCase) &&
               password == "Welkom123!";
    }

    public IReadOnlyList<MobileAppointment> GetPersonalPlanning()
    {
        return _appointments
            .OrderBy(a => a.StartTime)
            .ToList();
    }

    public MobileAppointment? GetAppointment(int id)
    {
        return _appointments.FirstOrDefault(a => a.Id == id);
    }
}
