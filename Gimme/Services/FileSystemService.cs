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


        public TryAsync<string> TryToReadAllTextAsync(string fromFile) =>
            TryAsync(() => File.ReadAllTextAsync(fromFile));

        public Try<T> TryToDeserialize<T>(string fromTextContent) where T : class
            => Try(() => JsonSerializer.Deserialize<T>(fromTextContent, jsonSerializerOptions));

        public Try<string> TryToSerialize<T>(T fromValue) where T : class
            => Try(() => JsonSerializer.Serialize<T>(fromValue, jsonSerializerOptions));

        public Option<T> ExceptionToOptionNoneOf<T>(Exception exception) where T : class => Option<T>.None;

        public async Task<Option<GimmeSettingsModel>> GetCurrentGimmeSettingsAsync()
            => await DoesFileExists(Constants.GIMME_SETTINGS_FILENAME)
                    .Right(fileInfo =>
                     TryToReadAllTextAsync(fromFile: fileInfo.FullName)
                        .Match
                        (
                            Succ: fileContent => TryToDeserialize<GimmeSettingsModel>(fromTextContent: fileContent)
                                                .Match(
                                                    Succ: settingsModel => Some(settingsModel),
                                                    Fail: ExceptionToOptionNoneOf<GimmeSettingsModel>
                                                    ),
                            Fail: ExceptionToOptionNoneOf<GimmeSettingsModel>
                        )
                    )
                    .Left(_ => Task.FromResult(Option<GimmeSettingsModel>.None));

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
    }

    public interface IFileSystemService
    {
        Either<Error, FileInfo> DoesFileExists(string relativePath);
        Validation<Error, FileInfo> DoesFileExistsValidation(string relativePath);
        bool FileExists(string relativePath);
        TryAsync<string> TryToReadAllTextAsync(string fromFile) ;
        Try<string> TryToSerialize<T>(T fromValue) where T : class;
        Try<T> TryToDeserialize<T>(string fromTextContent) where T : class;
        Either<Error, Unit> WriteAllTextToFile(string relativePath, string content);
        Task<Option<GimmeSettingsModel>> GetCurrentGimmeSettingsAsync();
    }
}
