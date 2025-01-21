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
	///
	/// Foreach case do both 'isDirectory: false' and 'isDirectory: true':
	/// - string.Empty
	/// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
	/// - (1, 2, 3) * "C:"
	/// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	/// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	/// - "C:abc.txt"
	/// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	/// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	/// </summary>
	[Theory]
    // - string.Empty
	[InlineData("", false)]
	[InlineData("", true)]
	// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
	[InlineData(@"/", false)]
	[InlineData(@"/", true)]
	[InlineData(@"\", false)]
	[InlineData(@"\", true)]
	[InlineData(@"//", false)]
	[InlineData(@"//", true)]
	[InlineData(@"\\", false)]
	[InlineData(@"\\", true)]
	[InlineData(@"///", false)]
	[InlineData(@"///", true)]
	[InlineData(@"\\\", false)]
	[InlineData(@"\\\", true)]
	// - (1, 2, 3) * "C:"
	[InlineData("C:", false)]
	[InlineData("C:", true)]
	[InlineData("C:C:", false)]
	[InlineData("C:C:", true)]
	[InlineData("C:C:C:", false)]
	[InlineData("C:C:C:", true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	[InlineData("C:" + "/", false)]
	[InlineData("C:" + "/", true)]
	[InlineData("C:" + "\\", false)]
	[InlineData("C:" + "\\", true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	[InlineData("/" + "C:", false)]
	[InlineData("/" + "C:", true)]
	[InlineData("\\" + "C:", false)]
	[InlineData("\\" + "C:", true)]
	// - "C:abc.txt"
	[InlineData("C:abc.txt", false)]
	[InlineData("C:abc.txt", true)]
	// - "C:abc."
	[InlineData("C:abc.", false)]
	[InlineData("C:abc.", true)]
	// - "C:abc"
	[InlineData("C:abc", false)]
	[InlineData("C:abc", true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	[InlineData("C:" + "/" + "abc.txt", false)]
	[InlineData("C:" + "/" + "abc.txt", true)]
	[InlineData("C:" + "\\" + "abc.txt", false)]
	[InlineData("C:" + "\\" + "abc.txt", true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc."
	[InlineData("C:" + "/" + "abc.", false)]
	[InlineData("C:" + "/" + "abc.", true)]
	[InlineData("C:" + "\\" + "abc.", false)]
	[InlineData("C:" + "\\" + "abc.", true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc"
	[InlineData("C:" + "/" + "abc", false)]
	[InlineData("C:" + "/" + "abc", true)]
	[InlineData("C:" + "\\" + "abc", false)]
	[InlineData("C:" + "\\" + "abc", true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	[InlineData("/" + "C:" + "abc.txt", false)]
	[InlineData("/" + "C:" + "abc.txt", true)]
	[InlineData("\\" + "C:" + "abc.txt", false)]
	[InlineData("\\" + "C:" + "abc.txt", true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc."
	[InlineData("/" + "C:" + "abc.", false)]
	[InlineData("/" + "C:" + "abc.", true)]
	[InlineData("\\" + "C:" + "abc.", false)]
	[InlineData("\\" + "C:" + "abc.", true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc"
	[InlineData("/" + "C:" + "abc", false)]
	[InlineData("/" + "C:" + "abc", true)]
	[InlineData("\\" + "C:" + "abc", false)]
	[InlineData("\\" + "C:" + "abc", true)]
	public void Root_Directory(
		string absolutePathString,
		bool isDirectory,
		string expectedNameNoExtension,
		string expectedExtensionNoPeriod,
		string expectedValue,
		string expectedNameWithExtension,
		int ancestoryDirectoryListCount,
		bool parentDirectoryShouldBeNull,
		bool rootDriveShouldBeNull,
		bool shouldBeRootDirectory)
	{
		var environmentProvider = new InMemoryEnvironmentProvider();
		
		var absolutePath = new IEnvironmentProvider.AbsolutePath(absolutePathString, isDirectory, environmentProvider);

        Assert.Equal(expectedNameNoExtension, absolutePath.NameNoExtension);
        Assert.Equal(expectedExtensionNoPeriod, absolutePath.ExtensionNoPeriod);
        Assert.Equal(expectedValue, absolutePath.Value);
        Assert.Equal(expectedNameWithExtension, absolutePath.NameWithExtension);
        Assert.Equal(ancestoryDirectoryListCount, absolutePath.GetAncestorDirectoryList().Count);

		if (parentDirectoryShouldBeNull)
			Assert.Null(absolutePath.ParentDirectory);
		else
			Assert.NotNull(absolutePath.ParentDirectory);

		if (rootDriveShouldBeNull)
			Assert.Null(absolutePath.RootDrive);
		else
			Assert.NotNull(absolutePath.RootDrive);

		if (shouldBeRootDirectory)
			Assert.True(absolutePath.IsRootDirectory);
		else
			Assert.False(absolutePath.IsRootDirectory);

		Assert.Equal(isDirectory, absolutePath.IsDirectory);

		// Insignificant assertions.
		Assert.NotNull(absolutePath.EnvironmentProvider);
		Assert.Equal(absolutePathString, absolutePath.ExactInput);
		Assert.Equal(PathType.AbsolutePath, absolutePath.PathType);
	}
}
