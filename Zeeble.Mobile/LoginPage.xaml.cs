using Newtonsoft.Json;
using Plugin.Firebase.CloudMessaging;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class LoginPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        public LoginPage()
        {
            InitializeComponent();
            SecureStorage.RemoveAll();
            _apiService = new RestApiService();
        }

        private async void Next_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ParentPhoneEntry.Text))
            {
                await DisplayAlert("Login", "Plesae enter parent phone number", "Ok");
                return;
            }

            if (ParentPhoneEntry.Text.Length < 10)
            {
                await DisplayAlert("Login", "Plesae enter valid parent phone number", "Ok");
                return;
            }

            var deviceToken = await SecureStorage.GetAsync("device-token");
            if (string.IsNullOrEmpty(deviceToken))
            {
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                deviceToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
                await SecureStorage.SetAsync("device-token", deviceToken);
            }

            var request = new
            {
                ParentPhone = ParentPhoneEntry.Text,
                DeviceToken = deviceToken,
            };

            LoginIndicator.IsVisible = true;
            LoginIndicator.IsRunning = true;
            await ParentPhoneEntry.HideSoftInputAsync(CancellationToken.None);
            var result = await _apiService.GetAsync($"api/auth/{ParentPhoneEntry.Text}/search");

            LoginIndicator.IsVisible = false;
            LoginIndicator.IsRunning = false;

            if (string.IsNullOrEmpty(result))
            {
                await DisplayAlert("Login", "Technical error occured. Make sure you are connected to interent and try again.", "Ok");
                return;
            }

            var searchResultModel = JsonConvert.DeserializeObject<SearchResponseModel>(result);

            if (searchResultModel.Code == 401)
            {
                await DisplayAlert("Login", searchResultModel.Message, "Ok");
                return;
            }

            ParentPhoneEntry.IsVisible = false;

            LabelName.Text = searchResultModel.FullName;
            LabelBatch.Text = searchResultModel.BatchName;
            LabelParentPhone.Text = $"+91 {ParentPhoneEntry.Text}";            
            FrameUserDetails.IsVisible = true;
            ButtonNext.IsVisible = false;

            ButtonLogin.IsVisible = true;            
            PinEntry.IsVisible = true;
            ConfirmPinEntry.IsVisible = searchResultModel.Code == 101 ? true : false;
        }

        private async void Login_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ParentPhoneEntry.Text))
            {
                await DisplayAlert("Login", "Plesae enter roll number", "Ok");
                return;
            }

            if (string.IsNullOrEmpty(PinEntry.Text))
            {
                await DisplayAlert("Login", "Plesae enter pin", "Ok");
                return;
            }

            if (ConfirmPinEntry.IsVisible && string.IsNullOrEmpty(ConfirmPinEntry.Text))
            {
                await DisplayAlert("Login", "Plesae enter confirm pin", "Ok");
                return;
            }

            if (ConfirmPinEntry.IsVisible && !PinEntry.Text.Equals(ConfirmPinEntry.Text))
            {
                await DisplayAlert("Login", "Pin and confirm pin must be same", "Ok");
                return;
            }
            await PinEntry.HideSoftInputAsync(CancellationToken.None);
            var deviceToken = await SecureStorage.GetAsync("device-token");
            var request = new
            {
                LoginId = ParentPhoneEntry.Text,
                Pin = PinEntry.Text,
                ConfirmPinEntry = ConfirmPinEntry.Text,
                DeviceToken = deviceToken
            };


            LoginIndicator.IsVisible = true;
            LoginIndicator.IsRunning = true;

            var tokenResult = await _apiService.PostAsync("api/auth/authorize", JsonConvert.SerializeObject(request));
            LoginIndicator.IsVisible = false;
            LoginIndicator.IsRunning = false;

            if (string.IsNullOrEmpty(tokenResult))
            {
                await DisplayAlert("Login", "Invalid email or password", "Ok");
                return;
            }

            var tokenModel = JsonConvert.DeserializeObject<TokenModel>(tokenResult);

            if (tokenModel.Code > 0)
            {
                await DisplayAlert("Login", tokenModel.Message, "Ok");
                return;
            }
            else
            {
                var settings = new SettingModel
                {
                    StandardId = tokenModel.StandardId,
                    TenantId = tokenModel.TenantId,
                    UserId = tokenModel.UserId,
                    StandardName = tokenModel.StandardName,
                    Token = tokenModel.Token,
                    Email = tokenModel.Email,
                    MobileNumber = tokenModel.MobileNumber,
                    Name = tokenModel.FullName,
                    BatchId = tokenModel.BatchId,
                    BatchName = tokenModel.BatchName
                };

                await SecureStorage.SetAsync("settings", JsonConvert.SerializeObject(settings));
                Application.Current.MainPage = new AppShell();

            }
        }
    }
}
