
using Zeeble.Mobile.ViewModels;

namespace Zeeble.Mobile
{
    public partial class SectionsPage : ContentPage
    {        
        public SectionsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            BindingContext = await SectionsViewModel.CreateAsync();
        }
    }
}
