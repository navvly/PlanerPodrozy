using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.IO;
using Planer_podróży.Models;

namespace Planer_podróży
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Trip> trips = new();
        
        private string filePath =
        Path.Combine(FileSystem.AppDataDirectory, "trips.json");
       
        private Trip selectedTrip;
        public MainPage()
        {
            InitializeComponent();

            tripList.ItemsSource = trips;

            endDate.MinimumDate = startDate.Date;

            _ = LoadTrips();
        }

        private void CloseDetailsClicked(object sender, EventArgs e)
        {
            detailsFrame.IsVisible = false;

            tripList.SelectedItem = null;

            tripName.Text = "";
            cityName.Text = "";
            budget.Text = "";

            startDate.Date = DateTime.Today;
            endDate.Date = DateTime.Today;

            selectedTrip = null;

            addButton.Text = "Dodaj podróż";
        }

        private async void OnAddTripClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tripName.Text) ||
                string.IsNullOrWhiteSpace(cityName.Text) ||
                string.IsNullOrWhiteSpace(budget.Text))
            {
                await DisplayAlert(
                    "Błąd",
                    "Uzupełnij wszystkie pola.",
                    "OK");

                return;
            }
            if (selectedTrip != null)
            {
                selectedTrip.Name = tripName.Text;
                selectedTrip.City = cityName.Text;
                selectedTrip.StartDate = startDate.Date;
                selectedTrip.EndDate = endDate.Date;
                selectedTrip.Budget = budget.Text;

                var temp = trips.ToList();
                trips.Clear();

                foreach (var t in temp)
                {
                    trips.Add(t);
                }

                tripList.ItemsSource = null;
                tripList.ItemsSource = trips;

                detailsName.Text = $"Nazwa: {selectedTrip.Name}";
                detailsCity.Text = $"Miasto: {selectedTrip.City}";
                detailsDate.Text = $"Data: {selectedTrip.StartDate:dd.MM.yyyy} - {selectedTrip.EndDate:dd.MM.yyyy}";
                detailsBudget.Text = $"Budżet: {selectedTrip.Budget} zł";

                selectedTrip = null;
                addButton.Text = "Dodaj podróż";

                await SaveTrips();

                await DisplayAlert(
                    "Sukces",
                    "Zmiany zostały zapisane.",
                    "OK");
            }
            else
            {
                Trip trip = new Trip
                {
                    Name = tripName.Text,
                    City = cityName.Text,
                    StartDate = startDate.Date,
                    EndDate = endDate.Date,
                    Budget = budget.Text
                };

                trips.Add(trip);

                await SaveTrips();

                await DisplayAlert(
                    "Sukces",
                    "Podróż została dodana!",
                    "OK");
            }

            tripName.Text = "";
            cityName.Text = "";
            budget.Text = "";

           
        }

        private async void TripSelected(object sender, SelectionChangedEventArgs e)
        {

            if (e.CurrentSelection.FirstOrDefault() is Trip trip)
            {
                selectedTrip = trip;

                addButton.Text = "Zapisz zmiany";

                tripName.Text = trip.Name;
                cityName.Text = trip.City;
                startDate.Date = trip.StartDate;
                endDate.Date = trip.EndDate;
                budget.Text = trip.Budget;


                detailsFrame.IsVisible = true;
                detailsName.Text = $"Nazwa: {trip.Name}";
                detailsCity.Text = $"Miasto: {trip.City}";
                detailsDate.Text = $"Data: {trip.StartDate:dd.MM.yyyy} - {trip.EndDate:dd.MM.yyyy}";
                detailsBudget.Text = $"Budżet: {trip.Budget} zł";
                detailsStatus.Text = $"Status: {GetTripStatus(trip)}";

                tripList.SelectedItem = null;
            }
        }
        private void StartDateChanged(object sender, DateChangedEventArgs e)

        {
            endDate.MinimumDate = startDate.Date;
        }

        private void BudgetChanged(object sender, TextChangedEventArgs e)
        {
            string numbersOnly = new string(
                e.NewTextValue?
                .Where(char.IsDigit)
                .ToArray());

            if (budget.Text != numbersOnly)
            {
                budget.Text = numbersOnly;
            }
        }

        private async Task SaveTrips()
        {
            string json = JsonSerializer.Serialize(trips);

            await File.WriteAllTextAsync(filePath, json);
        }

        private async Task LoadTrips()
        {
            if (!File.Exists(filePath))
                return;

            string json = await File.ReadAllTextAsync(filePath);

            var loadedTrips =
                JsonSerializer.Deserialize<List<Trip>>(json);

            if (loadedTrips == null)
                return;

            trips.Clear();

            foreach (var trip in loadedTrips)
            {
                trips.Add(trip);
            }

        }
        private string GetTripStatus(Trip trip)
        {
            DateTime today = DateTime.Today;

            if (today < trip.StartDate)
                return "Planowana";

            if (today > trip.EndDate)
                return "Zakończona";

            return "W trakcie";
        }
        private async void OnDeleteTripClicked(object sender, EventArgs e)


        {
            if (selectedTrip == null)
            {
                await DisplayAlert(
                    "Błąd",
                    "Najpierw wybierz podróż z listy.",
                    "OK");

                return;
            }

            bool confirm = await DisplayAlert(
    "Potwierdzenie",
    $"Czy na pewno chcesz usunąć podróż '{selectedTrip.Name}'?",
    "Tak",
    "Nie");

            if (!confirm)
            {
                return;
            }

            trips.Remove(selectedTrip);

            await SaveTrips();

            detailsFrame.IsVisible = false;

            tripName.Text = "";
            cityName.Text = "";
            budget.Text = "";

            detailsName.Text = "";
            detailsCity.Text = "";
            detailsDate.Text = "";
            detailsBudget.Text = "";
            detailsStatus.Text = "";
            selectedTrip = null;

            await DisplayAlert(
                "Sukces",
                "Podróż została usunięta.",
                "OK");
        }

       
        }

    }

