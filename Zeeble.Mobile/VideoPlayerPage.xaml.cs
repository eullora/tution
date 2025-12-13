using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class VideoPlayerPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private VideoModel _videoModel;
        public VideoPlayerPage(VideoModel videoModel)
        {
            InitializeComponent();            
            _videoModel = videoModel;
        }
        protected override void OnAppearing()
        {
            VideoPlayer.Source = _videoModel.Url;
            VideoPlayer.Play();
            base.OnAppearing();
        }

        private async void CloseButton_Clicked(object sender, EventArgs e)
        {
            VideoPlayer.Stop();
            VideoPlayer.Dispose();

            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}
