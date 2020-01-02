using System.Collections.Generic;

using Serilog.Sinks.SystemConsole.Themes;

namespace HelloCoreCommons.Serilog {
    public static class CustomConsoleThemes {
        public static AnsiConsoleTheme KunterBunt { get; } = new AnsiConsoleTheme(
            new Dictionary<ConsoleThemeStyle, string> {
                [ConsoleThemeStyle.Text] = "\x1b[38;5;0253m",
                [ConsoleThemeStyle.SecondaryText] = "\x1b[38;5;0246m",
                [ConsoleThemeStyle.TertiaryText] = "\x1b[38;5;0242m",
                [ConsoleThemeStyle.Invalid] = "\x1b[33;1m", // yellow: poisoned?
                [ConsoleThemeStyle.Null] = "\x1b[38;5;0038m",
                [ConsoleThemeStyle.Name] = "\x1b[38;5;0081m",
                [ConsoleThemeStyle.String] = "\x1b[38;5;0216m",
                [ConsoleThemeStyle.Number] = "\x1b[38;5;151m",
                [ConsoleThemeStyle.Boolean] = "\x1b[38;5;0038m",
                [ConsoleThemeStyle.Scalar] = "\x1b[38;5;0079m",
                [ConsoleThemeStyle.LevelVerbose] = "\x1b[37m", // white
                [ConsoleThemeStyle.LevelDebug] = "\x1b[32m", // green
                [ConsoleThemeStyle.LevelInformation] = "\x1b[36m", // cyan
                [ConsoleThemeStyle.LevelWarning] = "\x1b[33m", // yellow
                [ConsoleThemeStyle.LevelError] = "\x1b[35m", // pink
                [ConsoleThemeStyle.LevelFatal] = "\x1b[31m", // red
            });
    }
}
