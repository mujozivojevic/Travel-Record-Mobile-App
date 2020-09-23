using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecordApp.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelRecordApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var loc = CrossGeolocator.Current;
            loc.PositionChanged += Locator_PositionChanged;
            TimeSpan time = new TimeSpan(0,0,5);
            await loc.StartListeningAsync(time,100);

            var position = await loc.GetPositionAsync();

            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);

            locationsMaps.MoveToRegion(span);

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Post>();
                var posts = conn.Table<Post>().ToList();

                DisplayInMap(posts);

            }
           
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            var loc = CrossGeolocator.Current;
            loc.PositionChanged -= Locator_PositionChanged;

            loc.StopListeningAsync();
        }
        private void DisplayInMap(List<Post> posts)
        {
            foreach (var post in posts)
            {
                try
                { 
                    var position = new Xamarin.Forms.Maps.Position(post.Latitude, post.Longitude);

                    var pin = new Xamarin.Forms.Maps.Pin()
                    {
                        Type = Xamarin.Forms.Maps.PinType.SavedPin,
                        Position = position,
                        Label = post.VenueName,
                        Address = post.Address
                    };
                    locationsMaps.Pins.Add(pin); 
                }
                catch (NullReferenceException nre) { }
                catch (Exception ex) { }
            }
        }

        private void Locator_PositionChanged(object sender, PositionEventArgs e)
        {
            var center = new Xamarin.Forms.Maps.Position(e.Position.Latitude, e.Position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);

            locationsMaps.MoveToRegion(span);

        }
    }
}