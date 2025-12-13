using Newtonsoft.Json;
using System.Globalization;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class HomePage : ContentPage
    {
        public List<ExamGraphDataModel> ExamData { get; set; }
        public List<SubjectMarkModel> SubjectData { get; set; }
        public List<ExamListModel> ExamList { get; set; }
        public List<ExamGroup> GroupedExams { get; set; }

        private readonly IRestApiService _restApiService;
        public HomePage()
        {
            InitializeComponent();
            _restApiService = new RestApiService();
        }

        protected override async void OnAppearing()
        {
            if (!NetworkService.IsConnected)
            {
                await DisplayAlert("No Internet", "Please check your internet connection.", "OK");
                return;
            }

            await GetHomeDataFromServer();
            ChartLastExams.ItemsSource = ExamData;            
            ListViewAllExams.ItemsSource = GroupedExams;            
            TestChart.ItemsSource = SubjectData;

           
            base.OnAppearing();
        }

        private async Task GetHomeDataFromServer()
        {

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            var settingsData = await SecureStorage.GetAsync("settings");
            var settings = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            ExamData = new List<ExamGraphDataModel>();

            var data = await _restApiService.GetAsync($"api/analyze/{settings.UserId}/exams"); // get last 10 exams
            if (!string.IsNullOrEmpty(data))
            {
                ExamData.AddRange(JsonConvert.DeserializeObject<IEnumerable<ExamGraphDataModel>>(data));

                var breakDownList = new List<SubjectMarkModel>();
                foreach (var brk in ExamData)
                {
                    breakDownList.AddRange(brk.BreakDown);
                }
                SubjectData =
                [
                    .. breakDownList.GroupBy(gr => gr.SubjectName).Select(g => new SubjectMarkModel
                    {
                        SubjectName = g.Key,
                        Marks = (int) g.Average(av => av.Marks)
                    }),
                ];
            }

            ExamList = new List<ExamListModel>();

            var examListData = await _restApiService.GetAsync($"api/analyze/{settings.UserId}/exams/all");
            if (!string.IsNullOrEmpty(examListData))
            {
                ExamList.AddRange(JsonConvert.DeserializeObject<IEnumerable<ExamListModel>>(examListData));

                var grouped = ExamList
               .GroupBy(e => e.Month)
               .Select(g => new ExamGroup(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key), g.ToList()))
               .ToList();

                GroupedExams = new List<ExamGroup>(grouped);
            }

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            });
        }


    }
}
