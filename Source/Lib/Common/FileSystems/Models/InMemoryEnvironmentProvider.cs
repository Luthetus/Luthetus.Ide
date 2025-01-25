using System.Collections.Immutable;
using static Luthetus.Common.RazorLib.FileSystems.Models.IEnvironmentProvider;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class InMemoryEnvironmentProvider : IEnvironmentProvider
{
	public const string SafeRelativeDirectory = "Luthetus/";

    private readonly object _pathLock = new();

    public InMemoryEnvironmentProvider()
    {
        RootDirectoryAbsolutePath = new AbsolutePath("/", true, this);
        HomeDirectoryAbsolutePath = new AbsolutePath("/Repos/", true, this);
        ActualRoamingApplicationDataDirectoryAbsolutePath = new AbsolutePath("/AppData/Roaming/", true, this);
        ActualLocalApplicationDataDirectoryAbsolutePath = new AbsolutePath("/AppData/Local/", true, this);
        
        SafeRoamingApplicationDataDirectoryAbsolutePath = new AbsolutePath(
        	JoinPaths(ActualRoamingApplicationDataDirectoryAbsolutePath.Value, SafeRelativeDirectory),
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
        }
    }

    public AbsolutePath RootDirectoryAbsolutePath { get; }
    public AbsolutePath HomeDirectoryAbsolutePath { get; }
    public AbsolutePath ActualRoamingApplicationDataDirectoryAbsolutePath { get; }
    public AbsolutePath ActualLocalApplicationDataDirectoryAbsolutePath { get; }
    public AbsolutePath SafeRoamingApplicationDataDirectoryAbsolutePath { get; }
    public AbsolutePath SafeLocalApplicationDataDirectoryAbsolutePath { get; }
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

    public AbsolutePath AbsolutePathFactory(string path, bool isDirectory)
    {
        return new AbsolutePath(path, isDirectory, this);
    }

    public RelativePath RelativePathFactory(string path, bool isDirectory)
    {
        return new RelativePath(path, isDirectory, this);
    }
}