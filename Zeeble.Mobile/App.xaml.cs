using Zeeble.Mobile.CustomControls;
using Zeeble.Mobile.Services;

namespace Zeeble.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new ContentPage();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
            {
                if (view is CustomEntry)
                {
#if ANDROID
                    float[] outerRadii = { 10, 10, 10, 10, 10, 10, 10, 10 };
                    Android.Graphics.Drawables.Shapes.RoundRectShape roundRectShape = new Android.Graphics.Drawables.Shapes.RoundRectShape(outerRadii, null, null);
                    var shape = new Android.Graphics.Drawables.ShapeDrawable(roundRectShape);
                    shape.Paint.Color = Android.Graphics.Color.Gray;
                    shape.Paint.StrokeWidth = 3;
                    shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                    handler.PlatformView.Background = shape;
#elif IOS
                handler.PlatformView.Layer.BorderColor = UIKit.UIColor.Gray.CGColor;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
#endif
                }
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(IView.Background), (handler, view) =>
            {
                if (view is CustomPicker)
                {
#if ANDROID
                    float[] outerRadii = { 10, 10, 10, 10, 10, 10, 10, 10 };
                    Android.Graphics.Drawables.Shapes.RoundRectShape roundRectShape = new Android.Graphics.Drawables.Shapes.RoundRectShape(outerRadii, null, null);
                    var shape = new Android.Graphics.Drawables.ShapeDrawable(roundRectShape);
                    shape.Paint.Color = Android.Graphics.Color.Gray;
                    shape.Paint.StrokeWidth = 3;
                    shape.Paint.SetStyle(Android.Graphics.Paint.Style.Stroke);
                    handler.PlatformView.Background = shape;
#elif IOS
                handler.PlatformView.Layer.BorderColor = UIKit.UIColor.Gray.CGColor;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.RoundedRect;
#endif
                }
            });

            CheckUserLoggedIn();
        }
        private async void CheckUserLoggedIn()
        {

            if (!NetworkService.IsConnected)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("No Internet", "You are not connected to internet. Please check your connection.", "OK");

#if ANDROID
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif IOS
    // On iOS, Apple recommends not programmatically killing apps.
    // You can just throw an unhandled exception to crash intentionally (NOT recommended).
    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
#elif WINDOWS
    System.Environment.Exit(0);
#endif
                });
            }

            var settings = await SecureStorage.GetAsync("settings");

            if (string.IsNullOrEmpty(settings))
            {
                MainPage = new NavigationPage(new LoginPage());                
            }
            else
            {
                MainPage = new AppShell();
            }
        }
    }
}
