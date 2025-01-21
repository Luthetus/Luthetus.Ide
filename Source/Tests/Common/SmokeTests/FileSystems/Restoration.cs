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
	/// 
	/// Passing in the expected values is a pain:
	/// ````[InlineData("", false, "expectedNameNoExtension", "expectedExtensionNoPeriod", "expectedValue", "expectedNameWithExtension", ancestoryDirectoryListCount, parentDirectoryShouldBeNull, rootDriveShouldBeNull, shouldBeRootDirectory)]
	/// </summary>
	[Theory]

	// - string.Empty
	//
	[InlineData(/*absolutePathString*/"", /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"",  /*expectedValue*/"",  /*expectedNameWithExtension*/"",  /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"", /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	
	// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
	//
	[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/false, /*expectedNameNoExtension*/@"/", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"/", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/false, /*expectedNameNoExtension*/@"\", /*expectedExtensionNoPeriod*/@"\", /*expectedValue*/@"\", /*expectedNameWithExtension*/@"\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"\", /*expectedExtensionNoPeriod*/@"\", /*expectedValue*/@"\", /*expectedNameWithExtension*/@"\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/false, /*expectedNameNoExtension*/@"//", /*expectedExtensionNoPeriod*/@"//", /*expectedValue*/@"//", /*expectedNameWithExtension*/@"//", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"//", /*expectedExtensionNoPeriod*/@"//", /*expectedValue*/@"//", /*expectedNameWithExtension*/@"//", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/false, /*expectedNameNoExtension*/@"\\", /*expectedExtensionNoPeriod*/@"\\", /*expectedValue*/@"\\", /*expectedNameWithExtension*/@"\\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"\\", /*expectedExtensionNoPeriod*/@"\\", /*expectedValue*/@"\\", /*expectedNameWithExtension*/@"\\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"///", /*isDirectory*/false, /*expectedNameNoExtension*/@"///", /*expectedExtensionNoPeriod*/@"///", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"///", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"///", /*isDirectory*/true,  /*expectedNameNoExtension*/@"///", /*expectedExtensionNoPeriod*/@"///", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"///", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/false, /*expectedNameNoExtension*/@"\\\", /*expectedExtensionNoPeriod*/@"\\\", /*expectedValue*/@"\\\", /*expectedNameWithExtension*/@"\\\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/true,  /*expectedNameNoExtension*/@"\\\", /*expectedExtensionNoPeriod*/@"\\\", /*expectedValue*/@"\\\", /*expectedNameWithExtension*/@"\\\", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - (1, 2, 3) * "C:"
	//
	[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	//
	[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	//
	[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:abc.txt"
	//
	[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/true, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:abc."
	//
	[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/true, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:abc"
	//
	[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/true, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc."
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc"
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc."
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]

	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc"
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"expectedNameNoExtension", /*expectedExtensionNoPeriod*/"expectedExtensionNoPeriod", /*expectedValue*/"expectedValue", /*expectedNameWithExtension*/"expectedNameWithExtension", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
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
