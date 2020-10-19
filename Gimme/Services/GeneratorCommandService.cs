using System.Collections.Generic;
using System.Linq;
using Gimme.Models;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Gimme.Services
{
    public class GeneratorCommandService : IGeneratorCommandService
    {
        private readonly IFileSystemService fileSystemService;

        public GeneratorCommandService(IFileSystemService fileSystemService)
        {
            this.fileSystemService = fileSystemService;
        }
        public IEnumerable<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings) {
            return fromSettings
                .GeneratorsFiles
                .Map(fileSystemService.TryToReadAllText)
                .BindT(fileSystemService.TryToDeserialize<GeneratorModel>)
                .Succs();
        }


    }
    public interface IGeneratorCommandService
    {
        IEnumerable<GeneratorModel> GetGenerators(GimmeSettingsModel fromSettings);
    }
}