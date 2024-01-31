namespace Luthetus.Common.RazorLib.FileSystems.Models;

public static class PermittanceChecker
{
    public const string ERROR_PREFIX = "FILE_PERMISSION_ERROR:";

    public static void AssertDeletionPermitted(
        IEnvironmentProvider environmentProvider,
        string path,
        bool isDirectory)
    {
        var simplePath = new SimplePath(path, isDirectory);

        if (environmentProvider.ProtectedPathList.Contains(simplePath))
            throw NotDeletionPermittedExceptionFactory(path, isDirectory);

        // Double check obviously bad scenarios.
        // These should already be protected paths, but this is an extra check.
        {
            if (path == "/" || path == "\\" || string.IsNullOrWhiteSpace(path))
                throw NotDeletionPermittedExceptionFactory(path, isDirectory);
        }

        if (!environmentProvider.DeletionPermittedPathList.Contains(simplePath))
        {
            foreach (var deletionPermittedPath in environmentProvider.DeletionPermittedPathList)
            {
                if (deletionPermittedPath.IsDirectory)
                {
                    // Check if the path is encompassed by a directory with delete permission.
                    //
                    // The idea here being: if a directory is allowed to be deleted,
                    // then all files which are not protected are able to be deleted too.
                    if (path.StartsWith(deletionPermittedPath.AbsolutePath))
                        return;
                }
            }

            throw NotDeletionPermittedExceptionFactory(path, isDirectory);
        }
    }

    /// <summary>
    /// Beyond <see cref="SimplePath"/>, the app also uses <see cref="IAbsolutePath"/>.
    /// So, since there can be more than one directory separators for a filesystem,
    /// then here run the constructor for <see cref="IAbsolutePath"/> as a check
    /// that the two validated, and standardized strings do not match.
    /// </summary>
    public static bool IsRootOrHomeDirectory(
        SimplePath simplePath,
        IEnvironmentProvider environmentProvider)
    {
        var absolutePath = environmentProvider.AbsolutePathFactory(
            simplePath.AbsolutePath,
            simplePath.IsDirectory);

        if (absolutePath.Value == environmentProvider.RootDirectoryAbsolutePath.Value ||
            absolutePath.Value == environmentProvider.HomeDirectoryAbsolutePath.Value)
        {
            return true;
        }

        return false;
    }

    private static ApplicationException NotDeletionPermittedExceptionFactory(
        string path,
        bool isDirectory)
    {
        var entryTypeName = isDirectory
            ? "directory"
            : "file";

        return new ApplicationException(
            $"{ERROR_PREFIX} The {entryTypeName} with path '{path}' was not permitted to be deleted.");
    }
}
