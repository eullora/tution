using Newtonsoft.Json;
using Zeeble.Mobile.Models;

namespace Zeeble.Mobile
{
    public partial class ProfilePage : ContentPage
    {        
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var userData = await SecureStorage.GetAsync("settings");
            var userModel = JsonConvert.DeserializeObject<SettingModel>(userData);
            LabelName.Text = userModel.Name;

            var list = new List<string>
            {
                $"Parent Phone - {userModel.MobileNumber}",
                $"Batch - {userModel.BatchId.ToString()}",
                $"Standard - {userModel.StandardName}"
            };

            ListViewProfileDetails.ItemsSource = list;

            var listSettings = new List<string>
            {
                "Refresh system cache",
                "Delete downloaded files",
                $"Logout"
            };

            ListViewSettings.ItemsSource = listSettings;

            base.OnAppearing();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            SecureStorage.Remove("settings");
            //Application.Current.MainPage = new NavigationPage(new KSATLoginPage());
            
        }
    }

}
