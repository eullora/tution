using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class OnlineExamPage : ContentPage
    {
        private ICollection<ExamModel> _examListModel;
        private readonly IRestApiService _restApiService;
        private ExamModel _selected;
        public OnlineExamPage()
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
                _examListModel = new List<ExamModel>(list.Where(x => x.Mode == "Online"));
            }
        }
      
        private async void ListViewExams_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            ((ListView)sender).SelectedItem = null;
            _selected = e.SelectedItem as ExamModel;

            if (_selected.StatusId == 0)
            {
                if (Navigation.ModalStack.Count == 0)
                {
                    await Navigation.PushModalAsync(new InstructionPage(_selected));
                }
            }

            else if (_selected.StatusId == 1)
            {
                await DisplayAlert("Exam", "You have already completed this Exam. Your result will be notified soon", "Ok");
            }
            else
            {
                if (Navigation.ModalStack.Count == 0)
                {
                    await Navigation.PushModalAsync(new OnlineResultPage(_selected));
                }
            }

            ((ListView)sender).SelectedItem = null;
        }
    }
}
