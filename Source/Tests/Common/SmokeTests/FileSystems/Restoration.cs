using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

/// <summary>
/// A lot of changes to Luthetus.Common were made and the tests will be taken to this file
/// 1 by 1 to fix them / re-write them.
/// </summary>
public class Restoration
{
	/// <summary>
	/// In mass write assertions for input that deals largely with the 'root directory'.
	/// i.e.: input that results in 0 or 1 parent directories.
	///
	/// Note: "test.txt" with 'isDirectory: false' is included here, because for an absolute path, while we can presume
	///       that the parent directory is "/", this would make it a relative path.
	///       So the assertion needs to be made, "what do we do in this scenario".
	///
	/// Sort of a snapshot test to check for consistency.
	/// </summary>
	[Fact]
	public void Root_Directory()
	{
		// This list is unreliable since it is a comment,
		// but it helps visualize the cases in this single '[Fact]' since collapsible lines in the editor isn't implemented yet.
		// 
		// Foreach case do both 'isDirectory: false' and 'isDirectory: true':
		// - string.Empty
		// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
		// - (1, 2, 3) * "C:"
		// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
		// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
		// - "C:abc.txt"
		// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
		// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
		//
		// TODO: This could probably be made into a scripting language that generates unit tests.
	
		IEnvironmentProvider environmentProvider = new InMemoryEnvironmentProvider();
		
		string absolutePathString;
		bool isDirectory;
		
		// - string.Empty
		{
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
		{
			// - 1 * (DirectorySeparatorChar/AltDirectorySeparatorChar)
			{
				// - 1 * DirectorySeparatorChar)
				absolutePathString = string.Empty;
				{
					isDirectory = false;
					throw new NotImplementedException();
				}
				// - 1 * AltDirectorySeparatorChar
				{
					isDirectory = true;
					throw new NotImplementedException();
				}
			}
			
			// - 2 * (DirectorySeparatorChar/AltDirectorySeparatorChar)
			{
				// - 2 * DirectorySeparatorChar
				absolutePathString = string.Empty;
				{
					isDirectory = false;
					throw new NotImplementedException();
				}
				// - 2 * AltDirectorySeparatorChar
				{
					isDirectory = true;
					throw new NotImplementedException();
				}
			}
			
			// - 3 * (DirectorySeparatorChar/AltDirectorySeparatorChar)
			{
				// - 3 * DirectorySeparatorChar
				absolutePathString = string.Empty;
				{
					isDirectory = false;
					throw new NotImplementedException();
				}
				// - 3 * AltDirectorySeparatorChar
				{
					isDirectory = true;
					throw new NotImplementedException();
				}
			}
		}
		
		// - (1, 2, 3) * "C:"
		{
			// - 1 * "C:"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// - 2 * "C:"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// - 3 * "C:"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
		{
			// - "C:" + DirectorySeparatorChar
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// - "C:" + AltDirectorySeparatorChar
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
		{
			// - DirectorySeparatorChar + "C:"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// - AltDirectorySeparatorChar + "C:"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// - "C:abc.txt"
		{
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
		{
			// - "C:" + DirectorySeparatorChar + "abc.txt"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// - "C:" + AltDirectorySeparatorChar + "abc.txt"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
		// (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
		{
			// DirectorySeparatorChar + "C:" + "abc.txt"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
			// AltDirectorySeparatorChar + "C:" + "abc.txt"
			absolutePathString = string.Empty;
			{
				isDirectory = false;
				throw new NotImplementedException();
			}
			{
				isDirectory = true;
				throw new NotImplementedException();
			}
		}
		
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
