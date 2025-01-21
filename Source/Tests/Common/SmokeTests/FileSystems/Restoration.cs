using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

/// <summary>
/// A lot of changes to Luthetus.Common were made and the tests will be taken to this file
/// 1 by 1 to fix them / re-write them.
/// </summary>
public class Restoration
{
	/// <summary>
	/// In mass write assertions for 'Assert.Null(absolutePath.ParentDirectory);' cases.
	///
	/// Note: "test.txt" is included here, because for an absolute path, while we can presume
	///       that the parent directory is "/", this would make it a relative path.
	///       So the assertion needs to be made, "what do we do in this scenario".
	///
	/// Sort of a snapshot test to check for consistency.
	/// </summary>
	[Fact]
	public void Null_ParentDirectory()
	{
		IEnvironmentProvider environmentProvider = new InMemoryEnvironmentProvider();
		
		string absolutePathString;
		bool isDirectory;
		
		// ==================================================
		// ==================================================
		absolutePathString = string.Empty;
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
        
        // ==================================================
		// ==================================================
		absolutePathString = environmentProvider.DirectorySeparatorChar;
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
        
        // ==================================================
		// ==================================================
		absolutePathString = environmentProvider.AltDirectorySeparatorChar;
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		
		// ==================================================
		// ==================================================
		absolutePathString = "C:";
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
        
        // ==================================================
		// ==================================================
		var absolutePathString = "C:" + environmentProvider.DirectorySeparatorChar;
		bool isDirectory;
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
        
        // ==================================================
		// ==================================================
		absolutePathString = "C:" + environmentProvider.AltDirectorySeparatorChar;
		{
			isDirectory = false;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.False(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal(string.Empty, absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal(string.Empty, absolutePath.Value);
	        Assert.Equal(string.Empty, absolutePath.NameWithExtension);
	        Assert.False(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
		{
			isDirectory = true;
			var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString: string.Empty, isDirectory: true, environmentProvider);
	        Assert.Null(absolutePath.ParentDirectory);
	        Assert.Equal(string.Empty, absolutePath.ExactInput);
	        Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	        Assert.True(absolutePath.IsDirectory);
	        Assert.NotNull(absolutePath.EnvironmentProvider);
	        Assert.Equal(string.Empty, absolutePath.NameNoExtension);
	        Assert.Equal("/", absolutePath.ExtensionNoPeriod);
	        Assert.Null(absolutePath.RootDrive);
	        Assert.Equal("/", absolutePath.Value);
	        Assert.Equal("/", absolutePath.NameWithExtension);
	        Assert.True(absolutePath.IsRootDirectory);
	        Assert.Count(0, absolutePath.GetAncestorDirectoryList());
		}
            
        Assert.Equal("/test.txt", absolutePath.Value);
        
        Assert.Null(absolutePath.ParentDirectory);
    		
		throw new NotImplementedException();
	}
}
