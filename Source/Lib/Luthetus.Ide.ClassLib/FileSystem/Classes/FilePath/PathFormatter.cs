namespace Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;

public static class PathFormatter
{
    public static string FormatAbsoluteFilePathString(
        string absoluteFilePathString,
        char directorySeparatorChar,
        bool isDirectory)
    {
        if (absoluteFilePathString.StartsWith(directorySeparatorChar))
        {
            absoluteFilePathString = new string(absoluteFilePathString
                .Skip(1)
                .ToArray());
        }

        if (isDirectory)
        {
            if (!absoluteFilePathString.EndsWith(directorySeparatorChar))
            {
                absoluteFilePathString = absoluteFilePathString +
                                         directorySeparatorChar;
            }
        }
        else
        {
            if (absoluteFilePathString.EndsWith(directorySeparatorChar))
            {
                absoluteFilePathString = new string(absoluteFilePathString
                    .SkipLast(1)
                    .ToArray());
            }
        }

        return absoluteFilePathString;
    }
}