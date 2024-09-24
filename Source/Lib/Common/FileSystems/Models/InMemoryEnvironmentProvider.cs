using System.Collections.Immutable;
using static Luthetus.Common.RazorLib.FileSystems.Models.IEnvironmentProvider;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class InMemoryEnvironmentProvider : IEnvironmentProvider
{
    private readonly object _pathLock = new();

    public InMemoryEnvironmentProvider()
    {
        RootDirectoryAbsolutePath = new AbsolutePath("/", true, this);
        HomeDirectoryAbsolutePath = new AbsolutePath("/Repos/", true, this);
        TempDirectoryAbsolutePath = new AbsolutePath("/Temp/", true, this);
        
        ProtectedPathList = ProtectedPathList.Add(
            new(RootDirectoryAbsolutePath.Value,
            RootDirectoryAbsolutePath.IsDirectory));

        // TODO: Why is 'IsRootDirectory' being used here?...
		//       ...Meanwhile others like this use 'IsDirectory'. Typo?
		{
			// TODO: I will add the 'IsDirectory' version in addition to the existing, 'IsRootDirectory'...
	        //       ...I think the 'IsRootDirectory' one was a typo, but I don't want to take a risk
	        //       when it comes to protecting the home directory path.
	        //       In the end its harmless to have the path in the protected list as a directory and not as one.
	        ProtectedPathList = ProtectedPathList.Add(new(
	        	HomeDirectoryAbsolutePath.Value,
	            HomeDirectoryAbsolutePath.IsDirectory));
			
			// TODO: This is the 'IsRootDirectory' version. And 'IsRootDirectory' is 'false'. Remove this?
	        ProtectedPathList = ProtectedPathList.Add(new(
	        	HomeDirectoryAbsolutePath.Value,
	            HomeDirectoryAbsolutePath.IsRootDirectory));
        }
        
        ProtectedPathList = ProtectedPathList.Add(new(
        	TempDirectoryAbsolutePath.Value,
            TempDirectoryAbsolutePath.IsDirectory));

        // Redundantly hardcode some obvious cases for protection.
        {
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("/", true));
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("\\", true));
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("", true));
        }
    }

    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }
    public IAbsolutePath TempDirectoryAbsolutePath { get; }
    public string DriveExecutingFromNoDirectorySeparator { get; } = string.Empty;
    public ImmutableHashSet<SimplePath> DeletionPermittedPathList { get; private set; } = ImmutableHashSet<SimplePath>.Empty;
    public ImmutableHashSet<SimplePath> ProtectedPathList { get; private set; } = ImmutableHashSet<SimplePath>.Empty;

    public char DirectorySeparatorChar => '/';
    public char AltDirectorySeparatorChar => '\\';

    public bool IsDirectorySeparator(char character) =>
        character == DirectorySeparatorChar || character == AltDirectorySeparatorChar;

    public string GetRandomFileName() => Guid.NewGuid().ToString();

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
        lock (_pathLock)
        {
            var absolutePath = simplePath.AbsolutePath;

            if (absolutePath == "/" || absolutePath == "\\" || string.IsNullOrWhiteSpace(absolutePath))
                return;

            if (PermittanceChecker.IsRootOrHomeDirectory(simplePath, this))
                return;

            DeletionPermittedPathList = DeletionPermittedPathList.Add(simplePath);
        }
    }

    public void DeletionPermittedDispose(SimplePath simplePath)
    {
        lock (_pathLock)
        {
            DeletionPermittedPathList = DeletionPermittedPathList.Remove(simplePath);
        }
    }

    public void ProtectedPathsRegister(SimplePath simplePath)
    {
        lock (_pathLock)
        {
            ProtectedPathList = ProtectedPathList.Add(simplePath);
        }
    }
    
    public void ProtectedPathsDispose(SimplePath simplePath)
    {
        lock (_pathLock)
        {
            var absolutePath = simplePath.AbsolutePath;

            if (absolutePath == "/" || absolutePath == "\\" || string.IsNullOrWhiteSpace(absolutePath))
                return;

            if (PermittanceChecker.IsRootOrHomeDirectory(simplePath, this))
                return;

            ProtectedPathList = ProtectedPathList.Remove(simplePath);
        }
    }

    public IAbsolutePath AbsolutePathFactory(string path, bool isDirectory)
    {
        return new AbsolutePath(path, isDirectory, this);
    }

    public IRelativePath RelativePathFactory(string path, bool isDirectory)
    {
        return new RelativePath(path, isDirectory, this);
    }
}