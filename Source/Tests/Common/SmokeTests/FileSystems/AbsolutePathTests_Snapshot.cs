using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

/// <summary>
/// I'm not sure what I want to call this file.
///
/// I sort of want 2 'AbsolutePathTests.cs' at the moment.
///
/// One of them I want to just "use the more sensible input"
/// and the other I want to just permute all the unique input and assert what
/// it currently is.
///
/// By "unique input" I don't mean string equality.
///
/// I mean to say, given the implementation a certain method,
/// you should be able to visually see various "unique" situations that can occur.
///
/// Invoking the AbsolutePath constructor with "text.txt" vs "apple.fruits" are equal cases.
/// It is just a 'IdentifierToken' 'PeriodToken' 'IdentifierToken'.
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
	[InlineData(/*absolutePathString*/"", /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	
	// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
	//
	[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"///", /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/3, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"///", /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/2, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/3, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/2, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]

	// - (1, 2, 3) * "C:"
	//
	[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/false, /*expectedNameNoExtension*/"C:C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"C:C:", /*expectedNameWithExtension*/"C:C:", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"C:C:/", /*expectedNameWithExtension*/"C:C:/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	//
	[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	//
	[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	
	// - "C:abc.txt"
	//
	[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	
	// - "C:abc."
	//
	[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	
	// - "C:abc"
	//
	[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/" + "abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/" + "abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/" + "abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/" + "abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc."
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/true, /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc"
	//
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/C:abc.txt", /*expectedNameWithExtension*/"C:abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc.txt/", /*expectedNameWithExtension*/"C:abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/C:abc.txt", /*expectedNameWithExtension*/"C:abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc.txt/", /*expectedNameWithExtension*/"C:abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc."
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc./", /*expectedNameWithExtension*/"C:abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc./", /*expectedNameWithExtension*/"C:abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc"
	//
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc/", /*expectedNameWithExtension*/"C:abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc/", /*expectedNameWithExtension*/"C:abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
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
	
	/// <summary>
	/// I think I want the test above this: 'Root_Directory(...)'
	/// to be an exhaustive approach to the problem.
	///
	/// But, I'd still like to "single out" some of these cases.
	///
	/// I think the most "value" is to pick 1 '[InlineData(...)]'
	/// from each "group" and look into how it acts.
	/// </summary>
	[Fact]
	public void Root_Directory_Group1()
	{
		// 1- string.Empty
		//
		[InlineData(/*absolutePathString*/"", /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"",  /*expectedValue*/"",  /*expectedNameWithExtension*/"",  /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"", /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group2()
	{
		// 2- (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
		//
		[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"/",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"//",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"///", /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/3, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"///", /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/2, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\",   /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\\",  /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"/", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/false, /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"", /*ancestoryDirectoryListCount*/3, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/@"\\\", /*isDirectory*/true,  /*expectedNameNoExtension*/@"", /*expectedExtensionNoPeriod*/@"/", /*expectedValue*/@"///", /*expectedNameWithExtension*/@"/", /*ancestoryDirectoryListCount*/2, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group3()
	{
		// 3- (1, 2, 3) * "C:"
		//
		[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:",     /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:C:",   /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/false, /*expectedNameNoExtension*/"C:C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"C:C:", /*expectedNameWithExtension*/"C:C:", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:C:C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"C:C:/", /*expectedNameWithExtension*/"C:C:/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group4()
	{
		// 4- "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
		//
		[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "/",  /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/false, /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/", /*expectedNameWithExtension*/"", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\", /*isDirectory*/true,  /*expectedNameNoExtension*/"", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/", /*expectedNameWithExtension*/"/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group5()
	{
		// 5- (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
		//
		[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"/" + "C:",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/false, /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:", /*expectedNameWithExtension*/"C:", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:/", /*expectedNameWithExtension*/"C:/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group6()
	{
		// 6- "C:abc.txt"
		//
		[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group7()
	{
		// 7- "C:abc."
		//
		[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group8()
	{
		// 8- "C:abc"
		//
		[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
		[InlineData(/*absolutePathString*/"C:abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/0, /*parentDirectoryShouldBeNull*/true, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/true)]
	}
	
	[Fact]
	public void Root_Directory_Group9()
	{
		// 9- "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
		//
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/" + "abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/" + "abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/" + "abc.txt", /*expectedNameWithExtension*/"abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/" + "abc.txt/", /*expectedNameWithExtension*/"abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group10()
	{
		// 10- "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc."
		//
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc.", /*isDirectory*/true, /*expectedNameNoExtension*/"abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc./", /*expectedNameWithExtension*/"abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group11()
	{
		// 11- "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc"
		//
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "/" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/abc", /*expectedNameWithExtension*/"abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"C:" + "\\" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/abc/", /*expectedNameWithExtension*/"abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/false, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group12()
	{
		// 12- (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
		//
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/C:abc.txt", /*expectedNameWithExtension*/"C:abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc.txt",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc.txt/", /*expectedNameWithExtension*/"C:abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"txt", /*expectedValue*/"/C:abc.txt", /*expectedNameWithExtension*/"C:abc.txt", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.txt", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.txt", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc.txt/", /*expectedNameWithExtension*/"C:abc.txt/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group13()
	{
		// 13- (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc."
		//
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/" + "C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc.",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc./", /*expectedNameWithExtension*/"C:abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc.", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc.", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc./", /*expectedNameWithExtension*/"C:abc./", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	}
	
	[Fact]
	public void Root_Directory_Group14()
	{
		// 14- (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc"
		//
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"/" + "C:" + "abc",  /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc/", /*expectedNameWithExtension*/"C:abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/false, /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"", /*expectedValue*/"/C:abc", /*expectedNameWithExtension*/"C:abc", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
		[InlineData(/*absolutePathString*/"\\" + "C:" + "abc", /*isDirectory*/true,  /*expectedNameNoExtension*/"C:abc", /*expectedExtensionNoPeriod*/"/", /*expectedValue*/"/C:abc/", /*expectedNameWithExtension*/"C:abc/", /*ancestoryDirectoryListCount*/1, /*parentDirectoryShouldBeNull*/false, /*rootDriveShouldBeNull*/true, /*shouldBeRootDirectory*/false)]
	}
}
