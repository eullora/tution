using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class OnlineResultPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private readonly ExamModel _examModel;
        public OnlineResultPage(ExamModel examModel)
        {
            InitializeComponent();
            _apiService = new RestApiService();
            _examModel = examModel;
        }

        private async Task GetResultFromServer()
        {
            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                LabelTitle.Text = _examModel.Name;
                LabelTotal.Text = $"{_examModel.BreakDown.Sum(x => x.Marks)} / {_examModel.Marks}";
                ChartSubjectMarks.ItemsSource = _examModel.BreakDown;

            });

            var settingsData = await SecureStorage.GetAsync("settings");
            var settingsModel = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            var data = await _apiService.GetAsync($"api/exams/{_examModel.Id}/result/{settingsModel.UserId}");                

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });

            if (!string.IsNullOrEmpty(data))
            {
                var resultModel = JsonConvert.DeserializeObject<ResultModel>(data);
                var questionsModel = JsonConvert.DeserializeObject<IEnumerable<QuestionPaperModel>>(resultModel.Data);
                ListViewAnswerSheet.ItemsSource = questionsModel;
            }
            else
            {
                await DisplayAlert("Result", "Somethign went wrong. Please close this page and try again", "Ok");
            }
        }
        protected override async void OnAppearing()
        {
            await GetResultFromServer();
            base.OnAppearing();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if(Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}