using System.Collections.Generic;

using Serilog.Sinks.SystemConsole.Themes;

namespace HelloCore.SerilogExtensions {
    public class CustomConsoleTheme : AnsiConsoleTheme {
        public CustomConsoleTheme(IReadOnlyDictionary<ConsoleThemeStyle, string> styles) : base(styles) {
        }

        public static AnsiConsoleTheme KunterBunt { get; } = CustomConsoleThemes.KunterBunt;
    }
}