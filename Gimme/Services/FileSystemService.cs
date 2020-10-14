using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Models;

namespace Gimme.Services
{
    public class FileSystemService : IFileSystemService
    {
        private string CurrentDirectory => Environment.CurrentDirectory;

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public bool FileExists(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath));

        public async Task<GimmeSettingsModel> GetCurrentGimmeSettingsAsync()
        {
            var settingText = await File.ReadAllTextAsync(Constants.GIMME_SETTINGS_FILENAME);
            return JsonSerializer.Deserialize<GimmeSettingsModel>(settingText);
        }

        
        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="content"></param>
        public void WriteAllTextToFile(string relativePath, string content)
            => File.WriteAllText(Path.Combine(CurrentDirectory, relativePath), content);
    }

    public interface IFileSystemService
    {
        bool FileExists(string relativePath);
        void WriteAllTextToFile(string relativePath, string content);
        Task<GimmeSettingsModel> GetCurrentGimmeSettingsAsync();
    }
}
