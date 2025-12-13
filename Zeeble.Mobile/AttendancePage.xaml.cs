using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class AttendancePage : ContentPage
    {
        private readonly IRestApiService _apiService;
        public AttendenceRoot _attendenceRoot;
        private ICollection<AttendanceModel> _attendanceModel;
        private ICollection<AttendanceModel> _attendanceFilter;
        public AttendancePage()
        {
            InitializeComponent();
            _apiService = new RestApiService();
        }
        private async Task GetAttendanceFromServer()
        {
            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            var settingsData = await SecureStorage.GetAsync("settings");
            var settingsModel = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            var data = await _apiService.GetAsync($"api/attendance/{settingsModel.UserId}");
            if (string.IsNullOrEmpty(data))
            {
                return;
            }

            _attendenceRoot = JsonConvert.DeserializeObject<AttendenceRoot>(data);
            _attendanceModel = _attendenceRoot.Attendance;
            _attendanceFilter = _attendanceModel.Where(x => x.CheckInDate.Month == DateTime.Now.Month).ToList();
            var grouped = _attendanceFilter
                  .GroupBy(e => e.Month)
                  .Select(g => new AttendanceGroup(g.Key, g.ToList()))
                  .ToList();

            ListViewAttendance.ItemsSource = _attendanceFilter; //new List<AttendanceGroup>(grouped);

            var chartSource = new List<AttendanceChart>
            {
                new AttendanceChart{Title = "Present", Days = _attendenceRoot.PresentDaysInYear },
                new AttendanceChart{Title = "Absent", Days = _attendenceRoot.DaysInYear -  _attendenceRoot.PresentDaysInYear },
            };

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                OverViewChart.ItemsSource = chartSource;

                LabelTotalDaysInYear.Text = $"Total Days : {_attendenceRoot.DaysInYear}";
                LabelDaysPresentInYear.Text = $"Days Attended: {_attendenceRoot.PresentDaysInYear}";

                LabelTotalDaysInMonth.Text = _attendenceRoot.TotalDaysThisMonth.ToString();
                LabelPresentDaysInMonth.Text = _attendenceRoot.PresentDaysThisMonth.ToString();

                LabelAbsentDaysInMonth.Text = (_attendenceRoot.TotalDaysThisMonth - _attendenceRoot.PresentDaysThisMonth).ToString();
                var monthList = _attendanceModel.Select(d => d.Month).Distinct().ToList();
                MonthPicker.ItemsSource = monthList;
                MonthPicker.SelectedItem = monthList.FirstOrDefault();

            });
        }
        protected override async void OnAppearing()
        {
            if (!NetworkService.IsConnected)
            {
                await DisplayAlert("No Internet", "Please check your internet connection.", "OK");
                return;
            }

            await GetAttendanceFromServer();
            base.OnAppearing();
        }

        private void MonthPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (MonthPicker.SelectedIndex < 0)
                return;

            // Get the selected item
            string selectedItem = MonthPicker.Items[MonthPicker.SelectedIndex];
            _attendanceFilter = _attendanceModel.Where(x => x.Month == selectedItem).ToList();

            var grouped = _attendanceFilter
               .GroupBy(e => e.Month)
               .Select(g => new AttendanceGroup(g.Key, g.ToList()))
               .ToList();

            ListViewAttendance.ItemsSource = _attendanceFilter;

        }

    }

}
