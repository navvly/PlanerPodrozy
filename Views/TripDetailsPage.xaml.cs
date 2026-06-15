using Planer_podróży.Models;

namespace Planer_podróży;

public partial class TripDetailsPage : ContentPage
{
    public TripDetailsPage(Trip trip)
    {
        InitializeComponent();

        tripNameLabel.Text = trip.Name;
        cityLabel.Text = $"Miasto: {trip.City}";
        dateLabel.Text =
            $"Data: {trip.StartDate:dd.MM.yyyy} - {trip.EndDate:dd.MM.yyyy}";
        budgetLabel.Text = $"Budżet: {trip.Budget} zł";
    }
}