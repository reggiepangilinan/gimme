using System;
using Gimme.Models;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace Gimme.Extensions
{
    public static class All
    {
        public static Validation<Error, GimmeSettingsModel> MustExists(this Option<GimmeSettingsModel> gimmeSettings)
        => gimmeSettings
             .Match(Some: settings => Success<Error, GimmeSettingsModel>(settings),
                    None: () => Fail<Error, GimmeSettingsModel>(Error.New("😂 Can't read file gimmeSettings.json. Make sure you've initialized gimme or you're in a correct working directory."))
                   );                 
    }
}
