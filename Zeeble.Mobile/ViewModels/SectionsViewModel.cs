using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile.ViewModels
{
    public class SectionsViewModel : INotifyPropertyChanged
    {
        private readonly IRestApiService _restApiService;
        private ICollection<Category> _sections;

        public SectionsViewModel()
        {
            _restApiService = new RestApiService();
            NavigateCommand = new Command<Category>(OnNavigate);
        }

        public ICollection<Category> Sections
        {
            get => _sections;
            set
            {
                _sections = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public static async Task<SectionsViewModel> CreateAsync()
        {
            var viewModel = new SectionsViewModel();
            await viewModel.LoadCategoriesAsync();
            return viewModel;
        }

        private async Task LoadCategoriesAsync()
        {
            Sections = await GetCategories();
        }

        public async Task<ICollection<Category>> GetCategories()
        {

            var sectionCache = await SecureStorage.GetAsync("section-list");
            if (!string.IsNullOrEmpty(sectionCache))
            {
                return JsonConvert.DeserializeObject<ICollection<Category>>(sectionCache);
            }

            var settingData = await SecureStorage.GetAsync("settings");
            var settingsModel = JsonConvert.DeserializeObject<SettingModel>(settingData);

            var result = await _restApiService.GetAsync($"api/sections/{settingsModel.TenantId}");
            var categoryModel = JsonConvert.DeserializeObject<ICollection<Category>>(result);
            await SecureStorage.SetAsync("section-list", result);
            return categoryModel;
        }

        private async void OnNavigate(Category selectedCategory)
        {
            if (selectedCategory.Name == "Exams")
            {
                await Shell.Current.GoToAsync("exams");
            }
            else if (selectedCategory.Name == "Documents")
            {
                await Shell.Current.GoToAsync("documents");
            }
            else if (selectedCategory.Name == "Videos")
            {
                await Shell.Current.GoToAsync("videos");
            }
            else if (selectedCategory.Name == "Attendance")
            {
                await Shell.Current.GoToAsync("attendance");
            } 
            else if (selectedCategory.Name == "Quiz")
            {
                await Shell.Current.GoToAsync("quizhome");
            }
            else if (selectedCategory.Name == "Fees")
            {
                await Shell.Current.GoToAsync("fees");
            }
            else if (selectedCategory.Name == "FlashCards")
            {
                await Shell.Current.GoToAsync("flashcards");
            }
            else
            {
                return;
            }

        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

