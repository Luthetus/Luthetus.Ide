using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public static class PathHelper
{
    /// <summary>
    /// Given: "/Dir/Homework/math.txt" and "../Games/"<br/>
    /// Then: "/Dir/Games/"<br/><br/>
    /// Calculate an absolute-path-string by providing a starting AbsolutePath, then
    /// a relative-path-string from the starting position.
    /// </summary>
    public static string GetAbsoluteFromAbsoluteAndRelative(
        IAbsolutePath absolutePath,
        string relativePathString,
        IEnvironmentProvider environmentProvider)
    {
        var upperDirectoryString = relativePathString.Replace(
            environmentProvider.AltDirectorySeparatorChar,
            environmentProvider.DirectorySeparatorChar);

        var upperDirectoryCount = 0;
        var moveUpDirectoryToken = $"..{environmentProvider.DirectorySeparatorChar}";

        int indexOfUpperDirectory;

        while ((indexOfUpperDirectory = upperDirectoryString.IndexOf(
            moveUpDirectoryToken, StringComparison.InvariantCulture)) != -1)
        {
            upperDirectoryCount++;

            upperDirectoryString = upperDirectoryString.Remove(
                indexOfUpperDirectory,
                moveUpDirectoryToken.Length);
        }

        if (absolutePath.IsDirectory && string.IsNullOrWhiteSpace(upperDirectoryString))
            upperDirectoryCount--; // nearest shared ancestor is the directory itself

        var sharedAncestorDirectories = absolutePath.AncestorDirectoryBag
            .SkipLast(upperDirectoryCount)
            .ToArray();

        if (sharedAncestorDirectories.Length > 0)
        {
            var nearestSharedAncestor = sharedAncestorDirectories.Last();
            var nearestSharedAncestorAbsolutePathString = nearestSharedAncestor.Value;

            return nearestSharedAncestorAbsolutePathString + upperDirectoryString;
        }

        return environmentProvider.DirectorySeparatorChar + relativePathString;
    }

    /// <summary>
    /// Given: "/Dir/Homework/math.txt" and "/Dir/Games/"<br/>
    /// Then: "../Games/"<br/><br/>
    /// 
    /// Calculate an absolute-path-string by providing a starting AbsolutePath, then
    /// an ending AbsolutePath. The relative-path-string to travel from start to end, will
    /// be returned as a string.
    /// </summary>
    public static string GetRelativeFromTwoAbsolutes(
        IAbsolutePath startingPath,
        IAbsolutePath endingPath,
        IEnvironmentProvider environmentProvider)
    {
        var pathBuilder = new StringBuilder();
        var commonPath = startingPath.AncestorDirectoryBag.First().Value;

        if ((startingPath.ParentDirectory?.Value ?? string.Empty) ==
            (endingPath.ParentDirectory?.Value ?? string.Empty))
        {
            // TODO: Will this code break when the mounted drives are different, and parent directories share same name?

            // Use './' because they share the same parent directory.
            pathBuilder.Append($".{environmentProvider.DirectorySeparatorChar}");

            commonPath = startingPath.AncestorDirectoryBag.Last().Value;
        }
        else
        {
            // Use '../' but first calculate how many UpDir(s) are needed
            int limitingIndex = Math.Min(
                startingPath.AncestorDirectoryBag.Count,
                endingPath.AncestorDirectoryBag.Count);

            var i = 0;

            for (; i < limitingIndex; i++)
            {
                var startingPathAncestor = startingPath.AncestorDirectoryBag[i];
                var endingPathAncestor = endingPath.AncestorDirectoryBag[i];

                if (startingPathAncestor.NameWithExtension == endingPathAncestor.NameWithExtension)
                    commonPath = startingPathAncestor.Value;
                else
                    break;
            }

            var upDirCount = startingPath.AncestorDirectoryBag.Count - i;

            for (int appendUpDir = 0; appendUpDir < upDirCount; appendUpDir++)
            {
                pathBuilder.Append("../");
            }
        }

        var notCommonPath = new string(endingPath.Value.Skip(commonPath.Length).ToArray());

        return pathBuilder.Append(notCommonPath).ToString();
    }

    public static string CalculateNameWithExtension(
        string nameNoExtension,
        string extensionNoPeriod,
        bool isDirectory)
    {
        if (isDirectory)
        {
            return nameNoExtension + extensionNoPeriod;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(extensionNoPeriod))
            {
                return nameNoExtension;
            }
            else
            {
                return nameNoExtension + '.' + extensionNoPeriod;
            }
        }
    }
}
