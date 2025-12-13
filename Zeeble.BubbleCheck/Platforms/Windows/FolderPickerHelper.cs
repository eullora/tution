#if WINDOWS
using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using WinRT.Interop;
using MauiApp1;

public class FolderPickerService
{
    public async Task<string?> PickFolderAsync()
    {
        var picker = new FolderPicker();
        picker.SuggestedStartLocation = PickerLocationId.Desktop;
        picker.FileTypeFilter.Add("*");

        // This is key to making it work in a WinUI app:
        var hwnd = ((MauiWinUIWindow) App.Current.Windows[0].Handler.PlatformView).WindowHandle;
        InitializeWithWindow.Initialize(picker, hwnd);

        StorageFolder folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }
}
#endif