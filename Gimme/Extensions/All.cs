using System;
using Gimme.Models;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Gimme.Extensions
{
    public static class All
    {
        public static Validation<string, GimmeSettingsModel> MustExists(this Option<GimmeSettingsModel> gimmeSettings)
        => gimmeSettings
             .Match(Some: settings => Success<string, GimmeSettingsModel>(settings),
                    None: () => Fail<string, GimmeSettingsModel>("😂 Can't read file gimmeSettings.json. Make sure you've initialized gimme or you're in a correct working directory.")
                   );
    }
}
