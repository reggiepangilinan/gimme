using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Extensions
{
    public static class GimmeConsoleExtensions
    {
        public static ConsoleColor GetTextColor(IConsole console, TextColor textColor)
        {
            return textColor switch
            {
                TextColor.Success => ConsoleColor.Green,
                TextColor.Error => ConsoleColor.Red,
                TextColor.Warning => ConsoleColor.Yellow,
                TextColor.Info => ConsoleColor.Cyan,
                _ => console.ForegroundColor,
            };
        }

        public static IConsole WriteLineWithColor(this IConsole console, string message, TextColor textColor = TextColor.Default)
        {
            console.ForegroundColor = GetTextColor(console, textColor);
            console.WriteLine(message);
            console.ResetColor();
            return console;
        }

        public static IConsole WriteLineSuccess(this IConsole console, string message)
         => WriteLineWithColor(console, message, TextColor.Success);

        public static IConsole WriteLineError(this IConsole console, string message)
         => WriteLineWithColor(console, message, TextColor.Error);

        public static IConsole WriteLineError(this IConsole console, Error error)
         => WriteLineWithColor(console, error.Message, TextColor.Error);

        public static IConsole WriteLineWarning(this IConsole console, string message)
         => WriteLineWithColor(console, message, TextColor.Warning);

        public static IConsole WriteLineInfo(this IConsole console, string message)
         => WriteLineWithColor(console, message, TextColor.Info);

        public static IConsole WriteLineDefault(this IConsole console, string message)
         => WriteLineWithColor(console, message, TextColor.Default);

        public static Unit ToUnit(this IConsole console) => unit;

        public static IConsole AppendEmptyLine(this IConsole console)
        {
            console.WriteLine(string.Empty);
            return console;
        }

        public static IConsole SetForegroundColor(this IConsole console, TextColor textColor)
        {
            console.ForegroundColor = GetTextColor(console, textColor);
            return console;
        }

        public static Unit ResultTo(this Validation<Error, Lst<string>> validations, IConsole console) {
            return validations.Match(
                Succ: messages =>
                {
                    messages.Map(console.WriteLineSuccess);
                },
                Fail: errors =>
                {
                    errors.Map(console.WriteLineError);
                }
            );
        }
    }

    public enum TextColor
    {
        Default,
        Success,
        Warning,
        Error,
        Info
    }
}
