using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class OfflineResultPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private readonly ExamModel _examModel;
        public OfflineResultPage(ExamModel examModel)
        {
            InitializeComponent();
            _apiService = new RestApiService();
            _examModel = examModel;
        }

        protected override void OnAppearing()
        {
            
            ChartSubjectMarks.ItemsSource = _examModel.BreakDown;
            LabelTestName.Text = _examModel.Name;
            LabelTotal.Text = $"{_examModel.BreakDown.Sum(x=> x.Marks)} / {_examModel.Marks}";
            base.OnAppearing();
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void ButtonDownloadPaper_Clicked(object sender, EventArgs e)
        {
            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            var localFileName = await GetPaperFileFromRemoteToLocal();

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(localFileName)
            });
        }

        private async Task<string> GetPaperFileFromRemoteToLocal()
        {
            string fileName = Path.Combine(FileSystem.AppDataDirectory, $"Question_Paper_{_examModel.Id}.pdf");
            if (File.Exists(fileName))
            {
                return fileName;
            }

            var bytes = await _apiService.GetBytes($"api/documents/{_examModel.Id}/paper");
            await File.WriteAllBytesAsync(fileName, bytes);
            return fileName;
        }
    }
}