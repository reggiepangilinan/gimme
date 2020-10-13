using System;
using System.IO;

namespace Gimme.Services
{
    public class FileSystemService : IFileSystemService
    {
        private string CurrentDirectory => Environment.CurrentDirectory;

        public bool FileExists(string relativePath)
            => File.Exists(Path.Combine(CurrentDirectory, relativePath));
    }

    public interface IFileSystemService
    {
        bool FileExists(string relativePath);
    }
}
