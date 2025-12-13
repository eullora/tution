using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class SettingsPage : ContentPage
    {
        private readonly IRestApiService _apiService;
        public SettingsPage()
        {
            InitializeComponent();
             
        }
    }
}
