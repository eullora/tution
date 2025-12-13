using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class FeesPage : ContentPage
    {
        private readonly IRestApiService _restApiService;
        public FeesPage()
        {
            InitializeComponent();
            _restApiService = new RestApiService();            
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!NetworkService.IsConnected)
            {
                await DisplayAlert("No Internet", "Please check your internet connection.", "OK");
                return;
            }

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            await GetFeesFromServer();

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });
        }
        private async Task GetFeesFromServer()
        {
            var userData = await SecureStorage.GetAsync("settings");
            var userModel = JsonConvert.DeserializeObject<SettingModel>(userData);

            var data = await _restApiService.GetAsync($"api/fees/{userModel.UserId}");
            if (!string.IsNullOrEmpty(data))
            {                
                var model = JsonConvert.DeserializeObject<FeeModel>(data);
                var discount = model.Discount != null ? model.Discount.Value.ToString() : "None";
                LableName.Text = model.FullName;
                LableBatch.Text = model.BatchName;

                //var list = new List<string>
                //{                                        
                //    $"Total Fees: {model.TotalFee}",
                //    $"Discount: {discount}",
                //    $"Paid Amount: {model.PaidAmount}",
                //    $"Balance: {model.Balance}"
                //};
                //ListViewDetails.ItemsSource = list;

                ListViewPayments.ItemsSource = model.Payments;
               
            }
        }
    }   
}
