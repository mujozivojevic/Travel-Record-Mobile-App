using Plugin.Geolocator;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecordApp.Logic;
using TravelRecordApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewTravelPage : ContentPage
    {
        public NewTravelPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var locator = CrossGeolocator.Current;
            var position = await locator.GetPositionAsync();

            var venues = await VenueLogic.GetVenues(position.Latitude, position.Longitude);
            venueListView.ItemsSource = venues;

        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                var selectedValue = venueListView.SelectedItem as Venue;
                var firstCategory = selectedValue.categories.FirstOrDefault();

                Post post = new Post()
                {
                    Experience = experienceEntry.Text,
                    CategoryId = firstCategory.id,
                    CategoryName = firstCategory.name,
                    Address = selectedValue.location.address,
                    Distance = selectedValue.location.distance,
                    Latitude = selectedValue.location.lat,
                    Longitude = selectedValue.location.lng,
                    VenueName = selectedValue.name
                };


                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Post>();
                    int rows = conn.Insert(post);

                    if (rows > 0)
                        DisplayAlert("Success", "Experience was added", "Ok");
                    else
                        DisplayAlert("Failure", "Experience was not added", "Ok");
                }
            }
            catch (NullReferenceException nre)
            {

            }
            catch(Exception ex)
            {

            }
        }
    }
}