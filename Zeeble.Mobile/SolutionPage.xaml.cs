
namespace Zeeble.Mobile
{
    public partial class SolutionPage : ContentPage
    {
        private readonly string _html;
        public SolutionPage(string html)
        {
            InitializeComponent();
            _html = html;
        }

        protected override async void OnAppearing()
        {
            WebViewSolution.Source = new HtmlWebViewSource
            {
                Html = _html
            };

            base.OnAppearing();
        }
    }

}
