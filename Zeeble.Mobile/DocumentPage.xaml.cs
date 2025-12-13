using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class DocumentPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private List<DocumentModel> _documentModel;
        private DocumentModel _selected;

        public DocumentPage()
        {
            InitializeComponent();
            _apiService = new RestApiService();
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

            await GetDocumentsFromServer();
            ListViewDocuments.ItemsSource = _documentModel;

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });
        }

        private async Task GetDocumentsFromServer()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            var settingsModel = JsonConvert.DeserializeObject<SettingModel>(settingsData);            
            var data =  await _apiService.GetAsync($"api/documents/{settingsModel.BatchId}");

            if (!string.IsNullOrEmpty(data))
            {
                _documentModel = JsonConvert.DeserializeObject<List<DocumentModel>>(data);
            }
        }

        private async Task<string> GetPDFFileFromRemoteToLocal()
        {
            string fileName = Path.Combine(FileSystem.AppDataDirectory, $"{_selected.FileName}.pdf");
            if (File.Exists(fileName))
            {
                return fileName;
            }

            var bytes = await _apiService.GetBytes($"api/documents/{_selected.Id}/document");
            await File.WriteAllBytesAsync(fileName, bytes);
            return fileName;
        }

        private async void ListViewDocuments_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            ((ListView)sender).SelectedItem = null;

            _selected = e.SelectedItem as DocumentModel;

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            var localFileName = await GetPDFFileFromRemoteToLocal();

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(localFileName)
            });

            ((ListView)sender).SelectedItem = null;
        }
    }
}
