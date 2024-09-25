using System.Collections.Immutable;
using static Luthetus.Common.RazorLib.FileSystems.Models.IEnvironmentProvider;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalEnvironmentProvider : IEnvironmentProvider
{
	public const string SafeRelativeDirectory = "Luthetus/";

    private readonly object _pathLock = new();

    public LocalEnvironmentProvider()
    {
        RootDirectoryAbsolutePath = new AbsolutePath(
        	"/",
        	true,
        	this);

        HomeDirectoryAbsolutePath = new AbsolutePath(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            true,
            this);
            
        ActualRoamingApplicationDataDirectoryAbsolutePath = new AbsolutePath(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            true,
            this);
            
        SafeRoamingApplicationDataDirectoryAbsolutePath = new AbsolutePath(
            JoinPaths(ActualRoamingApplicationDataDirectoryAbsolutePath.Value, SafeRelativeDirectory),
            true,
            this);
            
        ActualLocalApplicationDataDirectoryAbsolutePath = new AbsolutePath(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            true,
            this);
        
        SafeLocalApplicationDataDirectoryAbsolutePath = new AbsolutePath(
            JoinPaths(ActualLocalApplicationDataDirectoryAbsolutePath.Value, SafeRelativeDirectory),
            true,
            this);

        ProtectedPathList = ProtectedPathList.Add(new(
        	RootDirectoryAbsolutePath.Value,
            RootDirectoryAbsolutePath.IsDirectory));

		ProtectedPathList = ProtectedPathList.Add(new(
        	HomeDirectoryAbsolutePath.Value,
            HomeDirectoryAbsolutePath.IsDirectory));
        
        ProtectedPathList = ProtectedPathList.Add(new(
        	ActualRoamingApplicationDataDirectoryAbsolutePath.Value,
            ActualRoamingApplicationDataDirectoryAbsolutePath.IsDirectory));
            
        ProtectedPathList = ProtectedPathList.Add(new(
        	ActualLocalApplicationDataDirectoryAbsolutePath.Value,
            ActualLocalApplicationDataDirectoryAbsolutePath.IsDirectory));
            
        ProtectedPathList = ProtectedPathList.Add(new(
        	SafeRoamingApplicationDataDirectoryAbsolutePath.Value,
            SafeRoamingApplicationDataDirectoryAbsolutePath.IsDirectory));
            
        ProtectedPathList = ProtectedPathList.Add(new(
        	SafeLocalApplicationDataDirectoryAbsolutePath.Value,
            SafeLocalApplicationDataDirectoryAbsolutePath.IsDirectory));

        // Redundantly hardcode some obvious cases for protection.
        {
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("/", true));
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("\\", true));
            ProtectedPathList = ProtectedPathList.Add(new SimplePath("", true));

            try
            {
                var entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

                if (entryAssembly is not null)
                {
                    var driveExecutingFrom = Path.GetPathRoot(entryAssembly.Location);

                    if (driveExecutingFrom is not null)
                    {
                        if (driveExecutingFrom.EndsWith('/') || driveExecutingFrom.EndsWith('\\'))
                        {
                            DriveExecutingFromNoDirectorySeparator = driveExecutingFrom[..^1];

                            ProtectedPathList = ProtectedPathList.Add(
                                new SimplePath(DriveExecutingFromNoDirectorySeparator + '/',
                                true));
                            
                            ProtectedPathList = ProtectedPathList.Add(
                                new SimplePath(DriveExecutingFromNoDirectorySeparator + '\\',
                                true));
                        }
                        else
                        {
                            DriveExecutingFromNoDirectorySeparator = driveExecutingFrom;
                            ProtectedPathList = ProtectedPathList.Add(new SimplePath(driveExecutingFrom, true));
                        }
                    }
                }
            }
            catch (Exception)
            {
                // This code is intended to be an extra level of caution.
                // it trys to add the current drive to the protected path list if possible.
            }
        }
    }

    public IAbsolutePath RootDirectoryAbsolutePath { get; }
    public IAbsolutePath HomeDirectoryAbsolutePath { get; }
	public IAbsolutePath SafeRoamingApplicationDataDirectoryAbsolutePath { get; }
	public IAbsolutePath SafeLocalApplicationDataDirectoryAbsolutePath { get; }
	public IAbsolutePath ActualRoamingApplicationDataDirectoryAbsolutePath { get; }
	public IAbsolutePath ActualLocalApplicationDataDirectoryAbsolutePath { get; }

    public string DriveExecutingFromNoDirectorySeparator { get; }

    public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
    public ImmutableHashSet<SimplePath> DeletionPermittedPathList { get; private set; } = ImmutableHashSet<SimplePath>.Empty;
    public ImmutableHashSet<SimplePath> ProtectedPathList { get; private set; } = ImmutableHashSet<SimplePath>.Empty;

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