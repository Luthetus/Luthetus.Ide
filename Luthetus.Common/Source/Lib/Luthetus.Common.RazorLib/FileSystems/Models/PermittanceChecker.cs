using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        if (environmentProvider.ProtectedPaths.Contains(simplePath))
            throw NotDeletionPermittedExceptionFactory(path, isDirectory);

        // Double check obviously bad scenarios.
        // These should already be protected paths, but this is an extra check.
        {
            if (path == "/" || path == "\\" || string.IsNullOrWhiteSpace(path))
                throw NotDeletionPermittedExceptionFactory(path, isDirectory);
        }

        if (!environmentProvider.DeletionPermittedPaths.Contains(simplePath))
            throw NotDeletionPermittedExceptionFactory(path, isDirectory);
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
