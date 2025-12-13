using Newtonsoft.Json;
using TM = System.Timers;
using Zeeble.Mobile.Services;
using Zeeble.Mobile.Models.Quiz;
using Zeeble.Mobile.Utilities;

namespace Zeeble.Mobile
{
    public partial class QuizPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        private int currentQuestionIndex = 0;
        private int questionsCount = 0;
        private TM.Timer questionTimer;

        private int timeLeft = 15;
        private double totalTime = 15.0;

        private int correctCount = 0;
        private int inCorrectCount = 0;
        private int fileIndex = 0;
        private bool _isAnswerTapped;

        private QuestionModel _currentQuestion;
        private List<QuestionModel> questionsModel;
        private List<int> _sourceFiles;
        QuizSettingModel _settings;

        private bool isFullScreen = false;
        private readonly int _chapterId;

        public QuizPage(int chapterId)
        {
            InitializeComponent();                        
            _apiService = new RestApiService();
            questionsModel = new List<QuestionModel>();
            _chapterId = chapterId;

            //QuestionWebView.Navigated += (s, e) =>
            //{                
            //    QuestionWebView.HeightRequest = 400;
            //};
        }

        private async Task DisplayAlertAndExit()
        {            
            await DisplayAlert("Quiz", "You are not connected to the internet. The app will now close.", "OK");
            Environment.Exit(0);
        }

        private async Task CheckInternetConnection()
        {
            var current = Connectivity.NetworkAccess;
            if (current != NetworkAccess.Internet)
            {
                await DisplayAlertAndExit();
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();            
            DisposeTimer();
            await CheckInternetConnection();

            _settings = await AppUtils.GetSettings();
            timeLeft = Convert.ToInt32(_settings.Interval);
            totalTime = _settings.Interval;

            fileIndex = 0;
            questionsCount = 0;
            currentQuestionIndex = 0;

            questionsModel = new List<QuestionModel>();

            var fileIdData = await _apiService.GetAsync($"api/quiz/{_chapterId}/files");
            await SecureStorage.SetAsync("file_ids", fileIdData);


            var fileIds = await SecureStorage.GetAsync("file_ids");
            _sourceFiles = JsonConvert.DeserializeObject<List<int>>(fileIds);

            await LoadQuestionsFromServer();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            if (Handler?.PlatformView is Android.Webkit.WebView androidWebView)
            {
                androidWebView.Settings.BuiltInZoomControls = true;
                androidWebView.Settings.DisplayZoomControls = false; 
            }
        }

        private void DisposeTimer()
        {
            if (questionTimer != null)
            {
                questionTimer.Stop();
                questionTimer.Dispose();
            }
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            DisposeTimer();
            await Navigation.PushModalAsync(new SettingsPage());
        }

        private async void OnAnswerClicked(object sender, EventArgs e)
        {
            DisposeTimer();
            
            if (_isAnswerTapped)
            {
                return;
            }

            _isAnswerTapped = true;
            var button = sender as Button;
            if (button != null)
            {
                string value = button.Text;

                var currQuest = questionsModel[currentQuestionIndex];
                FeedbackLabel.IsVisible = true;
                TimerProgressBar.IsVisible = false;

                if (currQuest.CorrectAnswer.ToLower() == value.ToLower())
                {
                    FeedbackLabel.Text = $"{value} is correct";
                    FeedbackLabel.TextColor = Colors.Green;
                    button.BackgroundColor = Colors.Green;
                    correctCount++;
                }
                else
                {
                    FeedbackLabel.Text = $"Wrong... Answer was {currQuest.CorrectAnswer.ToUpper()}";
                    FeedbackLabel.TextColor = Colors.Red;
                    button.BackgroundColor = Colors.Red;
                    inCorrectCount++;
                }

                QuizActivityLoader.IsVisible = true;
                QuizActivityLoader.IsRunning = true;

                await Task.Delay(3000);
                FeedbackLabel.IsVisible = false;

                currentQuestionIndex++;

                if (currentQuestionIndex >= questionsCount)
                {
                    fileIndex++;

                    await LoadQuestionsFromServer();
                }

                LoadNextQuestion();
            }
        }
        private async Task LoadQuestionsFromServer()
        {
            DisposeTimer();
            _isAnswerTapped = false;

            QuizActivityLoader.IsVisible = true;
            QuizActivityLoader.IsRunning = true;
            StackOptions.IsVisible = false;
            TimerProgressBar.IsVisible = false;

            if (fileIndex >= _sourceFiles.Count())
            {
                fileIndex = 0;
            }

            var index = _sourceFiles[fileIndex];
            var result = await _apiService.GetAsync($"api/quiz/{index}");            

            QuizActivityLoader.IsRunning = false;
            QuizActivityLoader.IsVisible = false;

            if (!string.IsNullOrEmpty(result))
            {
                questionsModel = JsonConvert.DeserializeObject<List<QuestionModel>>(result);
                questionsCount = questionsModel.Count();

                currentQuestionIndex = 0;
                timeLeft = Convert.ToInt32(_settings.Interval);

                LoadNextQuestion();
            }
            else
            {
                fileIndex = 0;
                currentQuestionIndex = 0;
            }

            LabelInfo.Text = $"Questions - {questionsCount} Current Index {currentQuestionIndex} || Source Files {_sourceFiles.Count()}. File Index {fileIndex}";

        }

        private void LoadNextQuestion()
        {
            isFullScreen = false;
            _currentQuestion = questionsModel[currentQuestionIndex];
            LabelSubject.Text = "";
            LabelInfo.Text = $"Questions - {questionsCount} Current Index {currentQuestionIndex} || Source Files {_sourceFiles.Count()}. File Index {fileIndex}";

            Dispatcher.Dispatch(() =>
            {
                FeedbackLabel.IsVisible = false;

                //byte[] imageBytes = Convert.FromBase64String(_currentQuestion.ImageData.Replace("data:image/png;base64,", ""));
                //QuestionImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
                //var source = $"<img src='{_currentQuestion.ImageData}'>";

                QuestionWebView.Source = new HtmlWebViewSource
                {
                    Html = AppUtils.QuizQuestion.Replace("#base64", _currentQuestion.ImageData)
                };

                LabelSubject.Text = _currentQuestion.SubjectName;

                StackOptions.IsVisible = true;
                TimerProgressBar.IsVisible = true;
                LabelCorrect.Text = $"{correctCount}";
                LabelInCorrect.Text = $"{inCorrectCount}";

                ButtonA.BackgroundColor = Colors.Gray;
                ButtonB.BackgroundColor = Colors.Gray;
                ButtonC.BackgroundColor = Colors.Gray;
                ButtonD.BackgroundColor = Colors.Gray;

                _isAnswerTapped = false;
                QuizActivityLoader.IsRunning = false;
                QuizActivityLoader.IsVisible = false;

                StartTimer();
            });
        }

        private void StartTimer()
        {
            DisposeTimer();
            TimerProgressBar.ProgressColor = Colors.Navy;
            timeLeft = Convert.ToInt32(_settings.Interval);
            TimerProgressBar.Progress = 1.0;
            questionTimer = new TM.Timer(1000);
            questionTimer.Elapsed += OnTimedEvent;
            questionTimer.Start();
        }
        private void OnTimedEvent(object sender, TM.ElapsedEventArgs e)
        {
            timeLeft--;
            double progress = timeLeft / totalTime;
            Dispatcher.Dispatch(() =>
            {
                TimerProgressBar.Progress = timeLeft / totalTime;

                if (timeLeft <= 5)
                {
                    TimerProgressBar.ProgressColor = Colors.Red;
                }
            });

            if (timeLeft == 0)
            {
                questionTimer.Stop();
                currentQuestionIndex++;
                Dispatcher.DispatchAsync(HandleTimeUp);
            }
        }



        private async Task HandleTimeUp()
        {
            FeedbackLabel.Text = $"Time's up! The correct answer was {_currentQuestion.CorrectAnswer}.";
            FeedbackLabel.TextColor = Colors.Red;
            FeedbackLabel.IsVisible = true;
            StackOptions.IsVisible = false;
            QuizActivityLoader.IsVisible = true;
            QuizActivityLoader.IsRunning = true;

            await Task.Delay(3000);
            FeedbackLabel.IsVisible = false;
            if (currentQuestionIndex >= questionsCount)
            {
                await LoadQuestionsFromServer();
            }

            LoadNextQuestion();
        }

        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }
    }
}
