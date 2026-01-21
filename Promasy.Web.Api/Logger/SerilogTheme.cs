using Serilog.Sinks.SystemConsole.Themes;

namespace Promasy.Web.App.Logger;

public static class PromasySerilogTheme
{
    public static readonly AnsiConsoleTheme Console = new(
        new Dictionary<ConsoleThemeStyle, string>
        {
            // ===== TEXT =====
            [ConsoleThemeStyle.Text] = "\e[37m", // light gray
            [ConsoleThemeStyle.SecondaryText] = "\e[90m", // dark gray
            [ConsoleThemeStyle.TertiaryText] = "\e[90m",

            // ===== LEVELS =====
            [ConsoleThemeStyle.LevelVerbose] = "\e[90m", // dark gray
            [ConsoleThemeStyle.LevelDebug] = "\e[36m", // cyan
            [ConsoleThemeStyle.LevelInformation] = "\e[37m", // light gray
            [ConsoleThemeStyle.LevelWarning] = "\e[33m", // yellow
            [ConsoleThemeStyle.LevelError] = "\e[31m", // red
            [ConsoleThemeStyle.LevelFatal] = "\e[1;31m", // bright red

            // ===== METADATA =====
            [ConsoleThemeStyle.Name] = "\e[36m", // cyan
            [ConsoleThemeStyle.String] = "\e[32m", // green
            [ConsoleThemeStyle.Number] = "\e[35m", // magenta
            [ConsoleThemeStyle.Boolean] = "\e[35m",
            [ConsoleThemeStyle.Null] = "\e[90m"
        }
    );
}