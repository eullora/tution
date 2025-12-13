
using MauiApp1.Models;
using Newtonsoft.Json;

namespace MauiApp1.Utils
{
    public static class Utility
    {
        public static SettingModel LoadSetting()
        {
            var appDirectory = AppContext.BaseDirectory;
            var filePath = Path.Combine(appDirectory, "settings.json");
            var data = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<SettingModel>(data);
        }
    }
}
