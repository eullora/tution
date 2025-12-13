using The49.Maui.BottomSheet;
using Zeeble.Mobile.Models;

namespace Zeeble.Mobile
{
    public partial class OmrSheet : BottomSheet
    {
        private readonly List<ExamPaperModel> _exampPaper;
        private readonly int _count;
        private Func<int , Task> _buttonClickAction;

        public OmrSheet(IEnumerable<ExamPaperModel> exampPaper, Func<int, Task> buttonClickAction)
        {
            _exampPaper = exampPaper.ToList();
            _count = exampPaper.Count();
            _buttonClickAction = buttonClickAction;
            InitializeComponent();
            GenerateMap();
        }        
        private void GenerateMap()
        {
            int rowCounter = 0;
            int columnCounter = 0;
            int buttonTextCounter = 1;

            GridOmr.RowDefinitions = new RowDefinitionCollection();
            var rowCount = (_exampPaper.Count() / 4) + 1;

            for (int i = 0; i <= rowCount; i++)
            {
                GridOmr.RowDefinitions.Add(new RowDefinition
                {
                    Height = new GridLength(1, GridUnitType.Star)
                });
            }

            for (int j = 0; j < _count; j++)
            {
                var button = new Button();
                button.Text = buttonTextCounter.ToString();
                button.TextColor = Colors.White;                                
                button.WidthRequest = 50.0;
                button.HeightRequest = 50.0;
                button.HorizontalOptions = LayoutOptions.Center;
                button.VerticalOptions = LayoutOptions.Center;
                button.Margin = new Thickness(3,3,3,3);
                button.BorderColor = Colors.White;
                button.BorderWidth = 1;
                button.CommandParameter = j;

                if (_exampPaper[j].UserAnswer == "S")
                {
                    button.BackgroundColor = Colors.Red;
                }
                else if (!string.IsNullOrEmpty(_exampPaper[j].UserAnswer))
                {
                    button.BackgroundColor = Colors.Green;
                }
                else
                {
                    button.BackgroundColor = Colors.Black;
                }

                Grid.SetRow(button, rowCounter);
                Grid.SetColumn(button, columnCounter);
                columnCounter = columnCounter + 1;
                if (columnCounter > 5)
                {
                    columnCounter = 0;
                    rowCounter = rowCounter + 1;
                }
                button.Clicked += async (sender, e) => await OnQuestionNumberButtonClicked(button);

                GridOmr.Children.Add(button);
                buttonTextCounter++;
            }
        }

        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            await DismissAsync();
        }

        private async Task OnQuestionNumberButtonClicked(Button  btn)
        {
            if (btn.BackgroundColor == Colors.Black) 
            {
                return;
            }

           await _buttonClickAction(Convert.ToInt32(btn.CommandParameter));
           await DismissAsync();
        }
    }
}
