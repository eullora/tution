using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class VideoPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private List<VideoModel> _videoModel;
        public VideoPage()
        {
            InitializeComponent();
            _apiService = new RestApiService();

            ListViewVideos.RefreshCommand = new Command(async () =>
            {
                Dispatcher.Dispatch(() =>
                {
                    ListViewVideos.IsRefreshing = true;
                });

                SecureStorage.Remove("video-data");
                await GetVideosFromServer();

                Dispatcher.Dispatch(() =>
                {
                    ListViewVideos.IsRefreshing = false;
                });

                ListViewVideos.ItemsSource = _videoModel;
            });

        }
        protected override async void OnAppearing()
        {
            if (!NetworkService.IsConnected)
            {
                await DisplayAlert("No Internet", "Please check your internet connection.", "OK");
                return;
            }

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = true;
                LoadingIndicator.IsVisible = true;
            });

            await GetVideosFromServer();
            ListViewVideos.ItemsSource = _videoModel;

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            });

            base.OnAppearing();
        }

        private async Task GetVideosFromServer()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            var settingsModel = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            var data = await _apiService.GetAsync($"api/streams/{settingsModel.BatchId}");                

            if (!string.IsNullOrEmpty(data))
            {
                _videoModel = JsonConvert.DeserializeObject<List<VideoModel>>(data);
            }
        }

        private async void ListViewVideos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            ((ListView)sender).SelectedItem = null;

            var mdl = e.SelectedItem as VideoModel;
            if (Navigation.ModalStack.Count == 0)
            {
                await Navigation.PushModalAsync(new VideoPlayerPage(mdl));
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
