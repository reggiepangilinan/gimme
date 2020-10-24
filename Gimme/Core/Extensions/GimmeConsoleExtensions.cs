using System;
using LanguageExt;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using McMaster.Extensions.CommandLineUtils;

namespace Gimme.Core.Extensions
{
    public static class GimmeConsoleExtensions
    {
        public static ConsoleColor GetTextColor(IConsole console, ConsoleTextColor textColor)
        {
            return textColor switch
            {
                ConsoleTextColor.Success => ConsoleColor.Green,
                ConsoleTextColor.Error => ConsoleColor.Red,
                ConsoleTextColor.Warning => ConsoleColor.Yellow,
                ConsoleTextColor.Info => ConsoleColor.Cyan,
                _ => console.ForegroundColor,
            };
        }

        public static IConsole WriteLineWithColor(this IConsole console, string message, ConsoleTextColor textColor = ConsoleTextColor.Default)
        {
            console.ForegroundColor = GetTextColor(console, textColor);
            console.WriteLine(message);
            console.ResetColor();
            return console;
        }

        public static IConsole WriteLineSuccess(this IConsole console, string message)
         => console.WriteLineWithColor(message, ConsoleTextColor.Success);

        public static IConsole WriteLineError(this IConsole console, string message)
         => console.WriteLineWithColor(message, ConsoleTextColor.Error);

        public static IConsole WriteLineError(this IConsole console, Error error)
         => console.WriteLineWithColor(error.Message, ConsoleTextColor.Error);

        public static IConsole WriteLineWarning(this IConsole console, string message)
         => console.WriteLineWithColor(message, ConsoleTextColor.Warning);

        public static IConsole WriteLineInfo(this IConsole console, string message)
         => console.WriteLineWithColor(message, ConsoleTextColor.Info);

        public static IConsole WriteLineDefault(this IConsole console, string message)
         => console.WriteLineWithColor(message, ConsoleTextColor.Default);

        public static Unit ToUnit(this IConsole console) => unit;

        public static IConsole AppendEmptyLine(this IConsole console)
        {
            console.WriteLine(string.Empty);
            return console;
        }

        public static IConsole SetForegroundColor(this IConsole console, ConsoleTextColor textColor)
        {
            console.ForegroundColor = GetTextColor(console, textColor);
            return console;
        }

        public static IConsole ResultTo(this Validation<Error, Lst<string>> validations, IConsole console)
        {
             validations.Match(
                Succ: messages =>
                {
                    messages.Map(console.WriteLineSuccess);
                },
                Fail: errors =>
                {
                    errors.Map(console.WriteLineError);
                }
            );
            return console;
        }

        public static IConsole ResultTo(this Either<Error, Lst<string>> either, IConsole console)
        {
            ResultTo(either.ToValidation(),console);
            return console;
        }

    }

    public enum ConsoleTextColor
    {
        Default,
        Success,
        Warning,
        Error,
        Info
    }
}
