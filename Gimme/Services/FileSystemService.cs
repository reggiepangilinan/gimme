using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Gimme.Core.Models;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Gimme.Services
{
    public class FileSystemService : IFileSystemService
    {
        private readonly JsonSerializerOptions jsonSerializerOptions;

        public FileSystemService(JsonSerializerOptions jsonSerializerOptions)
        {
            this.jsonSerializerOptions = jsonSerializerOptions;
        }

        private string CurrentDirectory => Environment.CurrentDirectory;

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public bool FileExists(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath));

        public Validation<Error, FileInfo> DoesFileExistsValidation(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath))
            ? Success<Error, FileInfo>(new FileInfo(Path.Combine(CurrentDirectory, relativePath)))
            : Fail<Error, FileInfo>(Error.New(ToFileNotFoundMessage(relativePath)));

        private static string ToFileNotFoundMessage(string relativePath)
             => $"😭 File not found - {relativePath}";

        public Either<Error, FileInfo> DoesFileExists(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath))
            ? Right<Error, FileInfo>(new FileInfo(Path.Combine(CurrentDirectory, relativePath)))
            : Left<Error, FileInfo>(Error.New(ToFileNotFoundMessage(relativePath)));

        public TryAsync<string> TryToReadAllTextAsync(string fromFile)
            => TryAsync(() => File.ReadAllTextAsync(fromFile));

        public Try<string> TryToReadAllText(string fromFile)
           => () => File.ReadAllText(fromFile);

        public Try<T> TryToDeserialize<T>(string fromTextContent) where T : class
            => () => JsonSerializer.Deserialize<T>(fromTextContent, jsonSerializerOptions);

        public Try<string> TryToSerialize<T>(T fromValue) where T : class
            => () => JsonSerializer.Serialize<T>(fromValue, jsonSerializerOptions);

        public Option<T> ExceptionToOptionNoneOf<T>(Exception exception) where T : class => Option<T>.None;

        public Option<GimmeSettingsModel> GetCurrentGimmeSettings()
         => DoesFileExists(Constants.GIMME_SETTINGS_FILENAME)
                    .Map(f => f.Name)
                    .Map(TryToReadAllText)
                    .BindT(TryToDeserialize<GimmeSettingsModel>)
                    .Right(yy => yy.Match(
                                          Succ: settingsModel => Some(settingsModel), 
                                          Fail: ExceptionToOptionNoneOf<GimmeSettingsModel>)
                                          )
                    .Left(_ => Option<GimmeSettingsModel>.None);


        /// <summary>
        /// Creates a new file, writes the specified string to the file, and then closes the file. If the target file already exists, it is overwritten.
        /// </summary>
        /// <param name="relativePath"></param>
        /// <param name="content"></param>
        public Either<Error, Unit> WriteAllTextToFile(string relativePath, string content)
        {
            try
            {
                File.WriteAllText(Path.Combine(CurrentDirectory, relativePath), content);
                return unit;
            }
            catch (Exception ex)
            {
                return Error.New(ex);
            }
        }


        public TryAsync<Unit> TryWriteAllTextToFileAsync(string relativePath, string content)
        =>
            TryAsync<Unit>(async () =>
            {
                await File.WriteAllTextAsync(Path.Combine(CurrentDirectory, relativePath), content);
                return unit;
            });
    }

    public interface IFileSystemService
    {
        Either<Error, FileInfo> DoesFileExists(string relativePath);
        Validation<Error, FileInfo> DoesFileExistsValidation(string relativePath);
        bool FileExists(string relativePath);
        TryAsync<string> TryToReadAllTextAsync(string fromFile);
        Try<string> TryToReadAllText(string fromFile);
        Try<string> TryToSerialize<T>(T fromValue) where T : class;
        Try<T> TryToDeserialize<T>(string fromTextContent) where T : class;
        Either<Error, Unit> WriteAllTextToFile(string relativePath, string content);
        TryAsync<Unit> TryWriteAllTextToFileAsync(string relativePath, string content);
        Option<GimmeSettingsModel> GetCurrentGimmeSettings();
    }
}
