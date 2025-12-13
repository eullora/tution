using Zeeble.Mobile.Models;

namespace Zeeble.Mobile
{
    public partial class InstructionPage : ContentPage
    {
        private readonly ExamModel _model;
        public InstructionPage(ExamModel model)
        {
            _model = model;
            InitializeComponent();
            LabelExamName.Text = _model.Name;

            LabelInstructios.Text = @"<div>            
               
                <div>
                    <p>
                        1. Physics and Chemistry section contains 15 questions.
                    </p>
                    <br/>
                    <p>
                       2. Biology section contains 20 questions.
                    </p> <br/>

                    <p>
                       3. Nature of Questions is Objective.
                    </p> <br/>
                    <p>
                         4. Each question has Four options (A), (B), (C) and (D). Only One of these four option is correct.
                    </p> 

                </div>
                <p><h5><br/>Marks distribution of questions is as follows.</h5></p>

                <div>
                    <p>
                        1. Physics and Chemistry section contains 15 questions.
                    </p>
                    <br/>
                    <p>
                       2. Biology section contains 20 questions.
                    </p> <br/>

                    <p>
                       3. Nature of Questions is Objective.
                    </p> <br/>
                    <p>
                         4. Each question has Four options (A), (B), (C) and (D). Only One of these four option is correct.
                    </p> 
                </div>   

             </div>";            

            //if(_model.StartDate.Date < DateTime.Now.Date)
            //{
                ButtonStart.IsVisible = true;
            //}
            //else
            //{
              //  LabelMessage.Text = $"You can start this exam will start on {_model.StartDate.ToString("dd-MMM-yyyy")}";
               // LabelMessage.IsVisible = true;
            //}
        }

        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
        }

        private async void ButtonStart_Clicked(object sender, EventArgs e)
        {
            if (Navigation.ModalStack.Count > 0)
            {
                await Navigation.PopModalAsync();
            }
            await Application.Current.MainPage.Navigation.PushModalAsync(new ExamPage(_model));

        }
    }
}
