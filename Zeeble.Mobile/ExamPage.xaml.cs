using Newtonsoft.Json;
using Zeeble.Mobile.Models;
using Zeeble.Mobile.Services;
using TM = System.Timers;

namespace Zeeble.Mobile
{
    public partial class ExamPage : ContentPage
    {
        private readonly IRestApiService _restApiService;
        private ExamModel _examModel;
        private List<ExamPaperModel> _examPaper;
        private ExamPaperModel _current;
        private int _currentIndex;
        private int _questionCount;
        private TM.Timer examTimer;

        private int totalMinutes = 0;
        private TimeSpan remainingTime;
        private SettingModel _settings;
        public ExamPage(ExamModel examModel)
        {
            InitializeComponent();
            _restApiService = new RestApiService();
            _examModel = examModel;
        }

        protected override bool OnBackButtonPressed()
        {
            DisplayAlert("Finish", "You must finish this test first. Click on Finish button.", "Ok");
            return true;
        }
        protected override async void OnAppearing()
        {
            totalMinutes = _examModel.Time;
            remainingTime = TimeSpan.FromMinutes(totalMinutes);
            if (_examPaper == null)
            {
                await LoadQuestionsFromServer();
                await Task.Delay(1000);
                _current = _examPaper[_currentIndex];
                ResetImage();
                StartTimer();
            }
        }
        private async void Next_Clicked(object sender, EventArgs e)
        {
            LabelMessage.Text = "";

            ButtonNext.BackgroundColor = Colors.Red;
            await Task.Delay(100);
            ButtonNext.BackgroundColor = Colors.White;


            if (string.IsNullOrEmpty(_current.UserAnswer))
            {
                LabelMessage.Text = "Please select answer";
                return;
            }

            MoveNext();
        }

        private void MoveNext()
        {
            _currentIndex = _currentIndex + 1;
            if (_currentIndex >= _questionCount)
            {
                LabelMessage.Text = "You have reached to the last question.";
                return;
            }
            _current = _examPaper[_currentIndex];
            ResetImage();
        }
        private async void Previous_Clicked(object sender, EventArgs e)
        {
            ButtonPrev.BackgroundColor = Colors.Red;
            await Task.Delay(100);
            ButtonPrev.BackgroundColor = Colors.White;

            _currentIndex = _currentIndex <= 0 ? 0 : _currentIndex - 1;
            _currentIndex = _currentIndex >= _questionCount ? _questionCount - 1 : _currentIndex;

            _current = _examPaper[_currentIndex];
            ResetImage();
        }

        private void ResetButtons()
        {
            Button_A.BackgroundColor = Colors.Gray;
            Button_B.BackgroundColor = Colors.Gray;
            Button_C.BackgroundColor = Colors.Gray;
            Button_D.BackgroundColor = Colors.Gray;
        }

        private void SetButtonColor(string option)
        {
            if (string.IsNullOrEmpty(option))
            {
                return;
            }
            else if (option == "A")
            {
                Button_A.BackgroundColor = Colors.Green;
            }

            else if (option == "B")
            {
                Button_B.BackgroundColor = Colors.Green;
            }

            else if (option == "C")
            {
                Button_C.BackgroundColor = Colors.Green;
            }

            else if (option == "D")
            {
                Button_D.BackgroundColor = Colors.Green;
            }
            else if (option == "S")
            {
                Button_A.BackgroundColor = Colors.Red;
                Button_B.BackgroundColor = Colors.Red;
                Button_C.BackgroundColor = Colors.Red;
                Button_D.BackgroundColor = Colors.Red;
            }
        }
        private void ButtonAnswer_Clicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var cmd = button.CommandParameter.ToString();

            if (_currentIndex >= _questionCount)
            {
                _currentIndex = _questionCount - 1;
            }

            _examPaper[_currentIndex].UserAnswer = button.CommandParameter.ToString();
            ResetButtons();
            SetButtonColor(cmd);
        }
        private async void Skip_Clicked(object sender, EventArgs e)
        {
            ButtonSkip.BackgroundColor = Colors.Navy;
            await Task.Delay(100);
            ButtonSkip.BackgroundColor = Colors.White;

            _current.UserAnswer = "S";
            SetButtonColor(_current.UserAnswer);
            MoveNext();
        }
        private async void ButtonOmr_Clicked(object sender, EventArgs e)
        {
            var omrPage = new OmrSheet(_examPaper, SetQuestionFromBottomSheet);
            omrPage.HasHandle = true;
            omrPage.HandleColor = Colors.Red;
            omrPage.HasBackdrop = true;
            await omrPage.ShowAsync(Window);
        }
        private async void ButtonEnd_Clicked(object sender, EventArgs e)
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            _settings = JsonConvert.DeserializeObject<SettingModel>(settingsData);
            ButtonEnd.IsEnabled = false;
            var answer = await DisplayAlert("Confirm", "Make sure you are connected to the internet. Do you want to end this test now?", "Yes", "No");

            if (answer)
            {
                DisposeTimer();

                ActivityLoader.IsVisible = true;
                ActivityLoader.IsRunning = true;

                var result = await SubmitPaper();

                ActivityLoader.IsVisible = false;
                ActivityLoader.IsRunning = false;

                if (!string.IsNullOrEmpty(result))
                {
                    await DisplayAlert("Thank You", "You have successfully submitted the your paper. Result will be notified on this application within 12 hours.", "Ok");

                    if (Application.Current.MainPage.Navigation.ModalStack.Count > 0)
                    {
                        await Task.Delay(3000);
                        await Application.Current.MainPage.Navigation.PopModalAsync();
                    }
                }
                else
                {
                    ButtonEnd.IsEnabled = true;
                    await DisplayAlert("Error", "Unable to submit your paper. Make sure you are connected to internet and try again.", "Ok");
                }
            }
        }
        private void OnTimedEvent(object sender, TM.ElapsedEventArgs e)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            Dispatcher.Dispatch(() =>
            {
                TimerProgressBar.Progress = remainingTime.TotalSeconds / (totalMinutes * 60);
                LabelTimer.Text = remainingTime.ToString(@"hh\:mm\:ss");

                if (remainingTime.TotalMinutes <= 5)
                {
                    TimerProgressBar.ProgressColor = Colors.Red;
                    LabelTimer.TextColor = Colors.Red;
                }
            });

            if (remainingTime.TotalSeconds <= 0)
            {
                DisposeTimer();
                Dispatcher.DispatchAsync(HandleTimeUp);
            }
        }
        private async Task LoadQuestionsFromServer()
        {
            ActivityLoader.IsVisible = true;
            ActivityLoader.IsRunning = true;
            var data = await _restApiService.GetAsync($"api/exams/{_examModel.Id}/paper");

            ActivityLoader.IsVisible = false;
            ActivityLoader.IsRunning = false;

            if (!string.IsNullOrEmpty(data))
            {
                var questions = JsonConvert.DeserializeObject<IEnumerable<ExamPaperModel>>(data);

                var subjects = questions.Select(x => x.SubjectName).Distinct();
                _examPaper = new List<ExamPaperModel>();

                foreach (var subject in subjects)
                {
                    _examPaper.AddRange(questions.Where(x => x.SubjectName == subject).OrderBy(g => Guid.NewGuid()));
                }

                _questionCount = _examPaper.Count;
            }
        }
        private async Task SetQuestionFromBottomSheet(int number)
        {
            await Task.Delay(500);
            _currentIndex = number;
            QuestionNumbers.Text = $"{(_currentIndex + 1)}/{_questionCount}";
            _current = _examPaper[_currentIndex];
            ResetButtons();

            byte[] imageBytes = Convert.FromBase64String(_current.ImageData.Replace("data:image/png;base64,", ""));
            QuestionImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            SetButtonColor(_current.UserAnswer);
        }
        private void StartTimer()
        {
            TimerProgressBar.ProgressColor = Colors.Navy;
            TimerProgressBar.Progress = 1.0;
            examTimer = new TM.Timer(1000);
            examTimer.Elapsed += OnTimedEvent;
            examTimer.Start();
        }
        private void DisposeTimer()
        {
            if (examTimer != null)
            {
                examTimer.Stop();
                examTimer.Dispose();
            }
        }
        private async Task<string> SubmitPaper()
        {
            var data = new
            {
                ExamId = _examModel.Id,
                _settings.UserId,
                ExamPaper = _examPaper.Select(x => new
                {
                    x.Id,
                    x.SubjectName,
                    x.UserAnswer
                })
            };

            var body = JsonConvert.SerializeObject(data);
            return await _restApiService.PostAsync($"api/exams/{_examModel.Id}/submit", body);
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            DisposeTimer();
        }
        private async Task HandleTimeUp()
        {
            await Task.Delay(3000);
            LabelMessage.Text = $"Time's up!. Please press 'Finish' button to submit your paper.";
            ButtonSkip.IsEnabled = false;
            ButtonPrev.IsEnabled = false;
            ButtonNext.IsEnabled = false;
            StackButtonHolder.IsEnabled = false;
            OmrButton.IsEnabled = false;
        }
        private void ResetImage()
        {
            Dispatcher.Dispatch(() =>
            {
                byte[] imageBytes = Convert.FromBase64String(_current.ImageData.Replace("data:image/png;base64,", "").Trim());
                QuestionImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                LabelMessage.Text = "";
                ResetButtons();
                SetButtonColor(_current.UserAnswer);
                SubjectName.Text = _current.SubjectName;
                QuestionNumbers.Text = $"{_currentIndex + 1}/{_questionCount}";
            });
        }
    }

}
