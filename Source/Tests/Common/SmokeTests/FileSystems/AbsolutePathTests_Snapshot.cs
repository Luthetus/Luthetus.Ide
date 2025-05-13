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
public class AbsolutePathTests_Snapshot
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
		
		var absolutePath = new AbsolutePath(absolutePathString, isDirectory, environmentProvider);

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
	
	/*
	string.Empty
	============
	
	The local filesystem was used
	
		// Directory
		{
			// var existsBefore = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.Directory.CreateDirectoryAsync(absolutePathString);
			// System.ArgumentException : The value cannot be an empty string. (Parameter 'path')
			
			// await fileSystemProvider.Directory.DeleteAsync(absolutePathString, false);
			// System.NullReferenceException : Object reference not set to an instance of an object.
			
			// await fileSystemProvider.Directory.CopyAsync(absolutePathString, "/");
			// System.NotImplementedException : The method or operation is not implemented.
			
			// await fileSystemProvider.Directory.MoveAsync(absolutePathString, "/");
			// System.NullReferenceException : Object reference not set to an instance of an object.
			
			// var directoryList = await fileSystemProvider.Directory.GetDirectoriesAsync(absolutePathString);
			// Console.WriteLine(directoryList.Length);
			// System.ArgumentException : The path is empty. (Parameter 'path')
			
			// var fileList = await fileSystemProvider.Directory.GetFilesAsync(absolutePathString);
			// System.ArgumentException : The path is empty. (Parameter 'path')
			
			// var fileSystemEntryList = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(absolutePathString);
			// Console.WriteLine(fileSystemEntryList.Count());
			// System.ArgumentException : The path is empty. (Parameter 'path')
			
			// var existsAfter = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsAfter: {existsAfter}");
		}
		
		// File
		{
			var existsBefore = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.File.DeleteAsync(absolutePathString);
			// System.NullReferenceException : Object reference not set to an instance of an object.
			
			// var destination = "/A.txt";
			// await fileSystemProvider.File.CopyAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
			// Console.WriteLine($"existsDestination: {existsDestination}");
		    // System.ArgumentException : The value cannot be an empty string. (Parameter 'sourceFileName')
		    
		    // var destination = "/A.txt";
		    // await fileSystemProvider.File.MoveAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
		    // Console.WriteLine($"existsDestination: {existsDestination}");
		    // System.NullReferenceException : Object reference not set to an instance of an object.
		    
		    // var dateTime = await fileSystemProvider.File.GetLastWriteTimeAsync(absolutePathString);
		    // Console.WriteLine(dateTime);
		    // System.ArgumentException : The path is empty. (Parameter 'path')
			
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.ArgumentException : The value cannot be an empty string. (Parameter 'path')
			
			// await fileSystemProvider.File.WriteAllTextAsync(absolutePathString, "apple");
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.NullReferenceException : Object reference not set to an instance of an object.
		        
		    // var existsAfter = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsAfter: {existsAfter}");
		}
		
		====================== "/"
		
		// Directory
		{
			// var existsBefore = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.Directory.CreateDirectoryAsync(absolutePathString);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException : The directory with path: '/' already exists
			
			// await fileSystemProvider.Directory.DeleteAsync(absolutePathString, false);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// await fileSystemProvider.Directory.CopyAsync(absolutePathString, "/");
			// System.NotImplementedException : The method or operation is not implemented.
			
			// await fileSystemProvider.Directory.MoveAsync(absolutePathString, "/");
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// var directoryList = await fileSystemProvider.Directory.GetDirectoriesAsync(absolutePathString);
			// Console.WriteLine(directoryList.Length);
			// existsBefore: True
			// 20
			// existsAfter: True
			
			// var fileList = await fileSystemProvider.Directory.GetFilesAsync(absolutePathString);
			// Console.WriteLine(fileList.Count());
			// existsBefore: True
			// 7
			// existsAfter: True
			
			// var fileSystemEntryList = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(absolutePathString);
			// Console.WriteLine(fileSystemEntryList.Count());
			// existsBefore: True
			// 27
			// existsAfter: True
			
			// var existsAfter = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsAfter: {existsAfter}");
		}
		
		// File
		{
			var existsBefore = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.File.DeleteAsync(absolutePathString);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// var destination = "/A.txt";
			// await fileSystemProvider.File.CopyAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
			// Console.WriteLine($"existsDestination: {existsDestination}");
		    // System.IO.DirectoryNotFoundException : Could not find a part of the path 'C:\'.
		    
		    // var destination = "/A.txt";
		    // await fileSystemProvider.File.MoveAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
		    // Console.WriteLine($"existsDestination: {existsDestination}");
		    // Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
		    
		    // var dateTime = await fileSystemProvider.File.GetLastWriteTimeAsync(absolutePathString);
		    // Console.WriteLine(dateTime);
		    // existsBefore: False
			// 1/21/2025 10:30:45 AM
			// existsAfter: False
			
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.UnauthorizedAccessException : Access to the path 'C:\' is denied.
			
			// await fileSystemProvider.File.WriteAllTextAsync(absolutePathString, "apple");
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.UnauthorizedAccessException: Access to the path 'C:\' is denied.
		        
		    var existsAfter = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			Console.WriteLine($"existsAfter: {existsAfter}");
		}
	*/
	
	/// <summary>
	/// Only can run [Theory] [InlineData(...)] as the entire batch at the moment,
	/// so I am going to copy some cases into here 1 by 1 to investigate how they should be handled.
	/// 
	/// From the perspective of the 'IFileSystemProvider',
	/// what I need to test is:
	/// - [ ] string.Empty
	///   - [ ] Delete permitted
	///     - [ ] Protected
	///     - [ ] NOT_protected
	///   - [X] NOT_delete-permitted
	///     - [X] Protected
	///     - [X] NOT_protected
	/// - [ ] file that exists
	///   - [ ] Delete permitted
	///     - [ ] Protected
	///     - [ ] NOT_protected
	///   - [ ] NOT_delete-permitted
	///     - [ ] Protected
	///     - [ ] NOT_protected
	/// - [ ] file that does not exist
	///   - [ ] Delete permitted
	///     - [ ] Protected
	///     - [ ] NOT_protected
	///   - [ ] NOT_delete-permitted
	///     - [ ] Protected
	///     - [ ] NOT_protected
	/// </summary>
	[Fact]
	public async Task SingleOut()
	{
		// string.Empty in 'new file' / 'new directory' ->
		// ===============================================
		//
		// isDirectory: false ->
		//     (this exception was caught)
		//     ERROR: System.IO.DirectoryNotFoundException:
		//     Could not find a part of the path 'C:\Users\hunte\Repos\Demos\BlazorCrudApp\BlazorCrudApp.ServerSide\Tests\'.
		//     at Microsoft.Win32.SafeHandles.SafeFileHandle.CreateFile(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
		//     at Microsoft.Win32.SafeHandles.SafeFileHandle.Open(String fullPath, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize, Nullable`1 unixCreateMode)
		//     at System.IO.File.OpenHandle(String path, FileMode mode, FileAccess access, FileShare share, FileOptions options, Int64 preallocationSize) at System.IO.File.WriteToFileAsync(String path, FileMode mode, String contents, Encoding encoding, CancellationToken cancellationToken)
		//     at Luthetus.Common.RazorLib.FileSystems.Models.LocalFileHandler.WriteAllTextAsync(String absolutePathString, String contents, CancellationToken cancellationToken)
		//     in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Common\FileSystems\Models\LocalFileHandler.cs:line 121
		//
		// isDirectory: true ->
		//     ???
		//     Seemingly nothing happened.
		//
		// 
		
		// I will test these with my own file system too not just the InMemory.
		// Need to do it in a way that it never gets committed I don't want any tests to have side effects.
		
		// Going to change the instances to local versions, run through these scenarios and see the outcomes.
		// will change back to in memory after.
		var environmentProvider = new InMemoryEnvironmentProvider();
		
		var fileSystemProvider = new InMemoryFileSystemProvider(
			environmentProvider, null, null);
		
		var absolutePathString = string.Empty;
		
		// Directory
		{
			// var existsBefore = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.Directory.CreateDirectoryAsync(absolutePathString);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException : The directory with path: '/' already exists
			
			// await fileSystemProvider.Directory.DeleteAsync(absolutePathString, false);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// await fileSystemProvider.Directory.CopyAsync(absolutePathString, "/");
			// System.NotImplementedException : The method or operation is not implemented.
			
			// await fileSystemProvider.Directory.MoveAsync(absolutePathString, "/");
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// var directoryList = await fileSystemProvider.Directory.GetDirectoriesAsync(absolutePathString);
			// Console.WriteLine(directoryList.Length);
			// existsBefore: True
			// 20
			// existsAfter: True
			
			// var fileList = await fileSystemProvider.Directory.GetFilesAsync(absolutePathString);
			// Console.WriteLine(fileList.Count());
			// existsBefore: True
			// 7
			// existsAfter: True
			
			// var fileSystemEntryList = await fileSystemProvider.Directory.EnumerateFileSystemEntriesAsync(absolutePathString);
			// Console.WriteLine(fileSystemEntryList.Count());
			// existsBefore: True
			// 27
			// existsAfter: True
			
			// var existsAfter = await fileSystemProvider.Directory.ExistsAsync(absolutePathString);
			// Console.WriteLine($"existsAfter: {existsAfter}");
		}
		
		// File
		{
			var existsBefore = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			Console.WriteLine($"existsBefore: {existsBefore}");
		
			// await fileSystemProvider.File.DeleteAsync(absolutePathString);
			// Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
			
			// var destination = "/A.txt";
			// await fileSystemProvider.File.CopyAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
			// Console.WriteLine($"existsDestination: {existsDestination}");
		    // System.IO.DirectoryNotFoundException : Could not find a part of the path 'C:\'.
		    
		    // var destination = "/A.txt";
		    // await fileSystemProvider.File.MoveAsync(absolutePathString, destination);
		    // var existsDestination = await fileSystemProvider.File.ExistsAsync(destination);
		    // Console.WriteLine($"existsDestination: {existsDestination}");
		    // Luthetus.Common.RazorLib.Exceptions.LuthetusCommonException: FILE_PERMISSION_ERROR: The directory with path '/' was not permitted to be deleted.
		    
		    // var dateTime = await fileSystemProvider.File.GetLastWriteTimeAsync(absolutePathString);
		    // Console.WriteLine(dateTime);
		    // existsBefore: False
			// 1/21/2025 10:30:45 AM
			// existsAfter: False
			
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.UnauthorizedAccessException : Access to the path 'C:\' is denied.
			
			// await fileSystemProvider.File.WriteAllTextAsync(absolutePathString, "apple");
			// var text = await fileSystemProvider.File.ReadAllTextAsync(absolutePathString);
			// Console.WriteLine(text);
			// System.UnauthorizedAccessException: Access to the path 'C:\' is denied.
		        
		    var existsAfter = await fileSystemProvider.File.ExistsAsync(absolutePathString);
			Console.WriteLine($"existsAfter: {existsAfter}");
		}
	}
}
