using Hospital.Mobile.Services;

namespace Hospital.Mobile.Views;

public partial class LoginPage : ContentPage
{
    private readonly HospitalMobileDataService _data;

    public LoginPage()
    {
        InitializeComponent();
        _data = HospitalMobileDataService.Instance;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim() ?? string.Empty;
        var password = PasswordEntry.Text ?? string.Empty;

        if (!_data.Login(email, password))
        {
            ErrorLabel.Text = "E-mailadres of wachtwoord is onjuist.";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;
        await Shell.Current.GoToAsync(nameof(HomePage));
    }
}
