
namespace Zeeble.Mobile.Services
{
    public static class NetworkService
    {
        public static bool IsConnected => Connectivity.NetworkAccess == NetworkAccess.Internet;
    }
}
