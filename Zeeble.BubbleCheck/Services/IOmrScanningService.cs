using MauiApp1.Models;
using MauiApp1.Utils;
using System.Diagnostics;

namespace MauiApp1.Services
{
    public interface IOmrScanningService
    {
        Task<string> ScanOmrSheetAsync(string enginePath, string imagePath, string templatePath);
        Task<string> WriteResultToFile(string filePath, string data);
        Task<string> WriteErrorToFile(string fileName, string data);
    }
    public class OmrScanningService : IOmrScanningService
    {
        public OmrScanningService()
        {
        }

        public async Task<string> ScanOmrSheetAsync(string enginePath, string imagePath, string templatePath)
        {
            var info = new ProcessStartInfo
            {
                FileName = "python",
                ArgumentList = { enginePath, imagePath, templatePath },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var prc = new Process
            {
                StartInfo = info
            };

            prc.Start();

            string output = await prc.StandardOutput.ReadToEndAsync();            
            await prc.WaitForExitAsync();
            return output;
        }

        public async Task<string> WriteResultToFile(string filePath, string data)
        {            
            await File.WriteAllTextAsync(filePath, data);
            return "Ok";
        }

        public async Task<string> WriteErrorToFile(string fileName, string data)
        {
            var fullFilePath = Path.Combine(fileName, fileName);
            await File.WriteAllTextAsync(fullFilePath, data);
            return "Ok";
        }
    }
}
