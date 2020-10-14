using System;
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
