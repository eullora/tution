using Newtonsoft.Json;
using System.Security.Cryptography;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class OfflineExamPage : ContentPage
    {
        private ICollection<ExamModel> _examListModel;
        private readonly IRestApiService _restApiService;
        private ExamModel _selected;
        public OfflineExamPage()
        {
            InitializeComponent();

            _restApiService = new RestApiService();
            _examListModel = new List<ExamModel>();
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
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            await GetTestsFromServer();
            ListViewExams.ItemsSource = _examListModel;

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });

            base.OnAppearing();
        }

        private async Task GetTestsFromServer()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            var settings = JsonConvert.DeserializeObject<SettingModel>(settingsData);            
            _examListModel = new List<ExamModel>();

            var data = await _restApiService.GetAsync($"api/exams/{settings.BatchId}/student/{settings.UserId}");
            if (!string.IsNullOrEmpty(data))
            {
                var list = JsonConvert.DeserializeObject<IEnumerable<ExamModel>>(data);
                _examListModel = new List<ExamModel>(list.Where(x =>x.Mode =="Offline"));                
            }
        }
        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null || e.CurrentSelection.Count == 0)
                return;

            var mdl = e.CurrentSelection.FirstOrDefault() as ExamModel;
            ListViewExams.SelectedItem = null;

            if (mdl.BreakDown == null)
            {
                await DisplayAlert("Exam", "Exam results are not avaialble yet. Your result will be notified soon", "Ok");
            }
            else
            {
                if (Navigation.ModalStack.Count == 0)
                {
                    await Navigation.PushModalAsync(new OfflineResultPage(mdl));
                }
            }

        }


        private async void ListViewExams_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            ((ListView)sender).SelectedItem = null;

            _selected = e.SelectedItem as ExamModel;

            if (_selected.BreakDown == null)
            {
                await DisplayAlert("Exam", "Exam results are not avaialble yet. Your result will be notified soon", "Ok");
            }
            else
            {
                if (Navigation.ModalStack.Count == 0)
                {
                    await Navigation.PushModalAsync(new OfflineResultPage(_selected));
                }
            }

            ((ListView)sender).SelectedItem = null;
        }

    }
}
