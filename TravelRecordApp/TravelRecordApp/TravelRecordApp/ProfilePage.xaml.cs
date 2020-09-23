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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using(SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                var postTable = conn.Table<Post>().ToList();

                var categories = postTable.OrderBy(w => w.CategoryId).Select(w => w.CategoryName).Distinct().ToList() ;


                Dictionary<string, int> categoriesCount = new Dictionary<string, int>();

                foreach (var category in categories)
                {
                    if (category != null)
                    {
                        var count = postTable.Where(w => w.CategoryName == category).ToList().Count;

                        categoriesCount.Add(category, count);
                    }


                }

                categoriesListView.ItemsSource = categoriesCount;

                postCountLabel.Text = postTable.Count.ToString();
            }
        }
    }
}