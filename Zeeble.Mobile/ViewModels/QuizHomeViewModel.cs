using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile.ViewModels
{
    public class QuizHomeViewModel : INotifyPropertyChanged
    {
        private readonly IRestApiService _restApiService;
        private ICollection<SubjectModel> _subjects;

        public QuizHomeViewModel()
        {
            _restApiService = new RestApiService();
            NavigateCommand = new Command<SubjectModel>(OnNavigate);
        }

        public ICollection<SubjectModel> Subjects
        {
            get => _subjects;
            set
            {
                _subjects = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public static async Task<QuizHomeViewModel> CreateAsync()
        {
            var viewModel = new QuizHomeViewModel();
            await viewModel.LoadCategoriesAsync();
            return viewModel;
        }

        private async Task LoadCategoriesAsync()
        {
            await GetSubjectsFromServer();
        }

        private async Task GetSubjectsFromServer()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            var settings = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            var cacheList = await SecureStorage.GetAsync("subject-list");
            if (!string.IsNullOrEmpty(cacheList))
            {
                _subjects = JsonConvert.DeserializeObject<List<SubjectModel>>(cacheList);
                return;
            }

            var data = await _restApiService.GetAsync($"api/{settings.TenantId}/subjects");
            if (!string.IsNullOrEmpty(data))
            {
                _subjects  = JsonConvert.DeserializeObject<List<SubjectModel>>(data);
                await SecureStorage.SetAsync("subject-list", string.IsNullOrEmpty(data) ? "" : data);
            }
        }

        private async void OnNavigate(SubjectModel selected)
        {
            
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

