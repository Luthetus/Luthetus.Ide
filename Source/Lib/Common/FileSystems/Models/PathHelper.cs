using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public static class PathHelper
{
    /// <summary>
    /// Given: "/Dir/Homework/math.txt" and "../Games/"<br/>
    /// Then: "/Dir/Games/"<br/><br/>
    /// |<br/>
    /// Calculate an absolute-path-string by providing a starting AbsolutePath, then
    /// a relative-path-string from the starting position.<br/>
    /// |<br/>
    /// Magic strings:<br/>
    ///     -"./" (same directory) token<br/>
    ///     -"../" (move up directory)<br/>
    ///     -If the relative path does not start with the previously
    ///      mentioned magic strings, then "./" (same directory) token is implicitly used.<br/>
    /// |<br/>
    /// This method accepts starting AbsolutePath that can be either a directory, or not.<br/>
    ///     -If provided a directory, then a "./" (same directory) token will target the
    ///      directory which the provided starting AbsolutePath is pointing to.<br/>
    ///     -If provided a file, then a "./" (same directory) token will target the
    ///      parent-directory in which the file is contained.<br/>
    ///     -If provided a directory, then a "../" (move up directory) token will target the
    ///      parent of the directory which the provided starting AbsolutePath is pointing to.<br/>
    ///     -If provided a file, then a "../" (move up directory) token will target the
    ///      parent directory of the parent directory in which the file is contained.<br/><br/>
    /// </summary>
    public static string GetAbsoluteFromAbsoluteAndRelative(
        IAbsolutePath absolutePath,
        string relativePathString,
        IEnvironmentProvider environmentProvider)
    {
        // Normalize the directory separator character
        relativePathString = relativePathString.Replace(
            environmentProvider.AltDirectorySeparatorChar,
            environmentProvider.DirectorySeparatorChar);

        // "../" is being called the 'moveUpDirectoryToken'
        var moveUpDirectoryCount = 0;
        var moveUpDirectoryToken = $"..{environmentProvider.DirectorySeparatorChar}";

        // Count all usages of "../",
        // and each time one is found: remove it from the relativePathString.
        while (relativePathString.StartsWith(moveUpDirectoryToken, StringComparison.InvariantCulture))
        {
            moveUpDirectoryCount++;
            relativePathString = relativePathString[moveUpDirectoryToken.Length..];
        }

        // "./" is being called the 'sameDirectoryToken'
        var sameDirectoryToken = $".{environmentProvider.DirectorySeparatorChar}";

        if (relativePathString.StartsWith(sameDirectoryToken))
        {
            if (moveUpDirectoryCount > 0)
            {
                // TODO: A filler expression is written here currently...
                //       ...but perhaps throwing an exception here is the way to go?
                //       |
                //       This if-branch implies that the relative path used
                //       "../" (move up directory) and
                //       "./" (same directory) tokens.
                _ = 0;
            }

            // Remove the same directory token text.
            relativePathString = relativePathString[sameDirectoryToken.Length..];
        }

        if (moveUpDirectoryCount > 0)
        {
            var sharedAncestorDirectories = absolutePath.GetAncestorDirectoryList()
                .SkipLast(moveUpDirectoryCount)
                .ToArray();
        
            if (sharedAncestorDirectories.Length > 0)
            {
                var nearestSharedAncestor = sharedAncestorDirectories.Last();
                var nearestSharedAncestorAbsolutePathString = nearestSharedAncestor;

                return nearestSharedAncestorAbsolutePathString + relativePathString;
            }
            else
            {
                // TODO: This case seems nonsensical?...
                //       ...It was written here originally,
                //       it is (2024-05-18) so this must have been here for a few months.
                //       |
                //       But, the root directory would always be a shared directory (I think)?
                return environmentProvider.DirectorySeparatorChar + relativePathString;
            }
        }
        else
        {
            // Side Note: A lack of both "../" (move up directory) and "./" (same directory) tokens,
            //            Implicitly implies: the "./" (same directory) token
            if (absolutePath.IsDirectory)
            {
                return absolutePath.Value + relativePathString;
            }
            else
            {
                if (absolutePath.ParentDirectory is null)
                    throw new NotImplementedException();
                else
                    return absolutePath.ParentDirectory + relativePathString;
            }
        }
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
        
        var startingPathAncestorDirectoryList = startingPath.GetAncestorDirectoryList();
        var endingPathAncestorDirectoryList = endingPath.GetAncestorDirectoryList();
        
        var commonPath = startingPathAncestorDirectoryList.First();

        if ((startingPath.ParentDirectory ?? string.Empty) ==
            (endingPath.ParentDirectory ?? string.Empty))
        {
            // TODO: Will this code break when the mounted drives are different, and parent directories share same name?

            // Use './' because they share the same parent directory.
            pathBuilder.Append($".{environmentProvider.DirectorySeparatorChar}");

            commonPath = startingPath.ParentDirectory;
        }
        else
        {
            // Use '../' but first calculate how many UpDir(s) are needed
            int limitingIndex = Math.Min(
                startingPathAncestorDirectoryList.Count,
                endingPathAncestorDirectoryList.Count);

            var i = 0;

            for (; i < limitingIndex; i++)
            {
                var startingPathAncestor = environmentProvider.AbsolutePathFactory(
                    startingPathAncestorDirectoryList[i],
                    true);

                var endingPathAncestor = environmentProvider.AbsolutePathFactory(
                    endingPathAncestorDirectoryList[i],
                    true);

                if (startingPathAncestor.NameWithExtension == endingPathAncestor.NameWithExtension)
                    commonPath = startingPathAncestor.Value;
                else
                    break;
            }

            var upDirCount = startingPathAncestorDirectoryList.Count - i;

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
                return nameNoExtension;
            else
                return nameNoExtension + '.' + extensionNoPeriod;
        }
    }
}
