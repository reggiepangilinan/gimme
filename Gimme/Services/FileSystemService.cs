using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Models;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

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

        public Validation<Error, Unit> FileExistsValidation(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath)) ? Success<Error, Unit>(unit) : Fail<Error, Unit>(Error.New($"😭 File not found - {relativePath}"));


        public async Task<Option<GimmeSettingsModel>> GetCurrentGimmeSettingsAsync()
        {
            if (this.FileExists(Constants.GIMME_SETTINGS_FILENAME))
            {
                var settingText = await File.ReadAllTextAsync(Constants.GIMME_SETTINGS_FILENAME);
                return Some(JsonSerializer.Deserialize<GimmeSettingsModel>(settingText));
            }
            return None;
        }


        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="content"></param>
        public Either<Error, Unit> WriteAllTextToFile(string relativePath, string content)
        {
            try {
                File.WriteAllText(Path.Combine(CurrentDirectory, relativePath), content);
                return unit;
            } catch(Exception ex) {
                return Error.New(ex);
            }
        }
    }

    public interface IFileSystemService
    {
        Validation<Error, Unit> FileExistsValidation(string relativePath);
        bool FileExists(string relativePath);
        Either<Error, Unit> WriteAllTextToFile(string relativePath, string content);
        Task<Option<GimmeSettingsModel>> GetCurrentGimmeSettingsAsync();
    }
}
