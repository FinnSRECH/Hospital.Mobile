namespace Hospital.Mobile.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnPlanningClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(PlanningPage));
    }
}
