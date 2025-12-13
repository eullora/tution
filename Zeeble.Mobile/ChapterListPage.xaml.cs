using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class ChapterListPage : ContentPage
    {
        private readonly SubjectModel _subject;
        private ICollection<ChapterModel> _chapters;
        private readonly IRestApiService _restApiService;
        
        public ChapterListPage(SubjectModel subject)
        {
            InitializeComponent();            
            _restApiService = new RestApiService();
            _subject = subject;
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection == null)
                return;
            
            var mdl = e.CurrentSelection.FirstOrDefault() as ChapterModel;
            CollectionViewChapters.SelectedItem = null;
            if(Navigation.ModalStack.Count == 0)
            {
                await Navigation.PushModalAsync(new QuizPage(mdl.Id));
            }            
        }

        protected override async void OnAppearing()
        {
            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            });

            await GetChaptersFromServer();
            CollectionViewChapters.ItemsSource = _chapters;

            Dispatcher.Dispatch(() =>
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            });

            base.OnAppearing();
        }

        private async Task GetChaptersFromServer()
        {
            var cacheList = await SecureStorage.GetAsync($"chapter-list-{_subject.Id}");
            if (!string.IsNullOrEmpty(cacheList))
            {
                _chapters = JsonConvert.DeserializeObject<List<ChapterModel>>(cacheList);
                if(_chapters.Count() > 0)
                {
                    return;
                }                
            }

            var data = await _restApiService.GetAsync($"api/{_subject.Id}/chapters");
            if (!string.IsNullOrEmpty(data))
            {
                _chapters = JsonConvert.DeserializeObject<List<ChapterModel>>(data);

                if (_chapters.Count() > 0)
                {
                    await SecureStorage.SetAsync($"chapter-list-{_subject.Id}", string.IsNullOrEmpty(data) ? "" : data);
                }
            }
        }
    }
}
