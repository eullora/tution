using Newtonsoft.Json;
using Zeeble.Mobile.Models.Quiz;

namespace Zeeble.Mobile.Utilities
{
    public static class AppUtils
    {
        public static async Task<QuizSettingModel> GetSettings()
        {
            var settingsData = await SecureStorage.GetAsync("settings");
            return JsonConvert.DeserializeObject<QuizSettingModel>(settingsData);
        }

        public static string QuizQuestion = $@"<!DOCTYPE html>
                        <html>
                        <head>
                            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0, maximum-scale=3.0, user-scalable=yes"">
                            <style>
                                body, html {{
                                    margin: 0;
                                    padding: 0;
                                    width: 100%;
                                    height: 100%;
                                }}
                                img {{
                                    display: block;
                                    max-width: 100%;
                                    height: auto;
                                }}
                            </style>
                        </head>
                <body>
                    <img src='#base64' />
                </body>
                </html>";
    }
}
