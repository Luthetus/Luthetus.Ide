namespace Luthetus.Ide.RazorLib.CommandLines.Models;

public class TerminalCommandFormatter
{
    public const string CHANGE_DIRECTORY_TARGET_FILE_NAME = "cd";

    public static FormattedCommand FormatChangeDirectory(string path) =>
        new FormattedCommand(CHANGE_DIRECTORY_TARGET_FILE_NAME, new[]
        {
            path,
        });
}
