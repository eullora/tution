using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.Provider;
using Android.OS;
using Plugin.Firebase.CloudMessaging;

namespace Zeeble.Mobile
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, 
        LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HandleIntent(Intent);
            CreateNotificationChannelIfNeeded();
            RequestNotificationPermission();            
        }

        private void RequestNotificationPermission()
        {
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (!notificationManager.AreNotificationsEnabled())
            {
                // Ask the user to enable notifications
                var intent = new Intent();
                intent.SetAction(Settings.ActionAppNotificationSettings);
                intent.PutExtra(Settings.ExtraAppPackage, PackageName);
                StartActivity(intent);
            }
        }
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }
        private static void HandleIntent(Intent intent)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }
        private void CreateNotificationChannelIfNeeded()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }

        private void CreateNotificationChannel()
        {
            var channelId = $"{PackageName}.general";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
            notificationManager.CreateNotificationChannel(channel);
            FirebaseCloudMessagingImplementation.ChannelId = channelId;
            FirebaseCloudMessagingImplementation.SmallIconRef = Resource.Drawable.icon;

        }
    }

}
