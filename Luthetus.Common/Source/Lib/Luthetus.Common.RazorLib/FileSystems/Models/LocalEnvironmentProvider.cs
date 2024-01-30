using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalEnvironmentProvider : IEnvironmentProvider
{
    private readonly object _specialPathLock = new();

    public LocalEnvironmentProvider()
    {
        RootDirectoryAbsolutePath = new AbsolutePath("/", true, this);

        HomeDirectoryAbsolutePath = new AbsolutePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true,
            this);

        ProtectedPaths = ProtectedPaths.Add(new SimplePath("/", true));
        ProtectedPaths = ProtectedPaths.Add(new SimplePath("\\", true));
        ProtectedPaths = ProtectedPaths.Add(new SimplePath("", true));
    }

    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    public ImmutableHashSet<SimplePath> DeletionPermittedPaths { get; private set; } = ImmutableHashSet<SimplePath>.Empty;
    public ImmutableHashSet<SimplePath> ProtectedPaths { get; private set; } = ImmutableHashSet<SimplePath>.Empty;

    public bool IsDirectorySeparator(char character) =>
        character == DirectorySeparatorChar || character == AltDirectorySeparatorChar;

    public string GetRandomFileName() => Path.GetRandomFileName();

    public string JoinPaths(string pathOne, string pathTwo)
    {
        if (IsDirectorySeparator(pathOne.LastOrDefault()))
            return pathOne + pathTwo;

        return string.Join(DirectorySeparatorChar, pathOne, pathTwo);
    }

    public void AssertDeletionPermitted(string path, bool isDirectory)
    {
        PermittanceChecker.AssertDeletionPermitted(this, path, isDirectory);
    }

    public void DeletionPermittedRegister(SimplePath simplePath)
    {
        lock (_specialPathLock)
        {
            DeletionPermittedPaths = DeletionPermittedPaths.Add(simplePath);
        }
    }

    public void DeletionPermittedDispose(SimplePath simplePath)
    {
        lock (_specialPathLock)
        {
            DeletionPermittedPaths = DeletionPermittedPaths.Remove(simplePath);
        }
    }

    public void ProtectedPathsRegister(SimplePath simplePath)
    {
        lock (_specialPathLock)
        {
            ProtectedPaths = ProtectedPaths.Add(simplePath);
        }
    }

    public void ProtectedPathsDispose(SimplePath simplePath)
    {
        lock (_specialPathLock)
        {
            ProtectedPaths = ProtectedPaths.Remove(simplePath);
        }
    }
}