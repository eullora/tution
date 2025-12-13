using CommunityToolkit.Maui;
using Maui.PDFView;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Platform;
using The49.Maui.BottomSheet;
using Plugin.Firebase.Android;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Shared;
using Syncfusion.Maui.Toolkit.Hosting;

namespace Zeeble.Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {            
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices()
                .UseBottomSheet()                                
                .UseMauiPdfView()   
                .ConfigureSyncfusionToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("NotoSerif-Bold.ttf", "NotoSerifBold");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                    fonts.AddFont("Poppins-SemiBold.ttf", "PoppinsSemibold");
                    fonts.AddFont("Poppins-Regular.ttf", "Poppins");
                    fonts.AddFont("MaterialIconsOutlined-Regular.otf", "Material");
                });

            builder.Services.AddSingleton<MaterialFontMarker>();

#if __ANDROID__
            ImageHandler.Mapper.PrependToMapping(nameof(Microsoft.Maui.IImage.Source), (handler, view) => PrependToMappingImageSource(handler, view));
#endif

            return builder.Build();
        }


#if __ANDROID__
        public static void PrependToMappingImageSource(IImageHandler handler, Microsoft.Maui.IImage image)
        {
            handler.PlatformView?.Clear();
        }
#endif

        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android => android.OnCreate((activity, state) => CrossFirebase.Initialize(activity, state, CreateCrossFirebaseSettings())));
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            
            return new CrossFirebaseSettings(isAuthEnabled: true, isCloudMessagingEnabled: true);
        }

    }
}
