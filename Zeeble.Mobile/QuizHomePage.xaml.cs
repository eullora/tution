using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;
using Zeeble.Mobile.ViewModels;

namespace Zeeble.Mobile
{
    public partial class QuizHomePage : ContentPage
    {
        private ICollection<SubjectModel> _subjectItems;
        private readonly IRestApiService _restApiService;
        public QuizHomePage()
        {
            InitializeComponent();
            _restApiService = new RestApiService();
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return; 

            var selectedItem = e.CurrentSelection.FirstOrDefault() as SubjectModel;
            if (selectedItem != null)
            {
                CollectionViewSubjects.SelectedItem = null;
                await Navigation.PushAsync(new ChapterListPage(selectedItem));
            }
        }

        protected override async void OnAppearing()
        {
            if (!NetworkService.IsConnected)
            {
                await DisplayAlert("No Internet", "Please check your internet connection.", "OK");
                return;
            }

            await GetSubjectsFromServer();
            CollectionViewSubjects.ItemsSource = _subjectItems;
            base.OnAppearing();
        }

        private async Task GetSubjectsFromServer()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            var settings = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            var cacheList = await SecureStorage.GetAsync($"subject-list");
            if (!string.IsNullOrEmpty(cacheList))
            {
                _subjectItems = JsonConvert.DeserializeObject<List<SubjectModel>>(cacheList);
                if (_subjectItems.Count() > 0)
                {
                    return;
                }                
            }

            var data = await _restApiService.GetAsync($"api/{settings.TenantId}/subjects");
            if (!string.IsNullOrEmpty(data))
            {
                _subjectItems = JsonConvert.DeserializeObject<List<SubjectModel>>(data);

                if (_subjectItems.Count() > 0)
                {
                    await SecureStorage.SetAsync("subject-list", string.IsNullOrEmpty(data) ? "" : data);
                }
            }
        }
    }
}
