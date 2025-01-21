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
	[InlineData(string.Empty, isDirectory: false)]
	[InlineData(string.Empty, isDirectory: true)]
	// - (1, 2, 3) * (DirectorySeparatorChar/AltDirectorySeparatorChar)
	[InlineData(@"/", isDirectory: false)]
	[InlineData(@"/" isDirectory: true)]
	[InlineData(@"\", false)]
	[InlineData(@"\", true)]
	[InlineData(@"//", isDirectory: false)]
	[InlineData(@"//", isDirectory: true)]
	[InlineData(@"\\", isDirectory: false)]
	[InlineData(@"\\", isDirectory: true)]
	[InlineData(@"///", isDirectory: false)]
	[InlineData(@"///", isDirectory: true)]
	[InlineData(@"\\\", isDirectory: false)]
	[InlineData(@"\\\", isDirectory: true)]
	// - (1, 2, 3) * "C:"
	[InlineData("C:", isDirectory: false)]
	[InlineData("C:", isDirectory: true)]
	[InlineData("C:C:", isDirectory: false)]
	[InlineData("C:C:", isDirectory: true)]
	[InlineData("C:C:C:", isDirectory: false)]
	[InlineData("C:C:C:", isDirectory: true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	[InlineData("C:" + '/', isDirectory: false)]
	[InlineData("C:" + '/', isDirectory: true)]
	[InlineData("C:" + "\\", isDirectory: false)]
	[InlineData("C:" + "\\", isDirectory: true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	[InlineData('/' + "C:", isDirectory: false)]
	[InlineData('/' + "C:", isDirectory: true)]
	[InlineData("\\" + "C:", isDirectory: false)]
	[InlineData("\\" + "C:", isDirectory: true)]
	// - "C:abc.txt"
	[InlineData("C:abc.txt", isDirectory: false)]
	[InlineData("C:abc.txt", isDirectory: true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	[InlineData("C:" + '/' + "abc.txt", isDirectory: false)]
	[InlineData("C:" + '/' + "abc.txt", isDirectory: true)]
	[InlineData("C:" + "\\" + "abc.txt", isDirectory: false)]
	[InlineData("C:" + "\\" + "abc.txt", isDirectory: true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	[InlineData('/' + "C:" + "abc.txt", isDirectory: false)]
	[InlineData('/' + "C:" + "abc.txt", isDirectory: true)]
	[InlineData("\\" + "C:" + "abc.txt", isDirectory: false)]
	[InlineData("\\" + "C:" + "abc.txt", isDirectory: true)]
	public void Root_Directory(string absolutePathString, bool isDirectory)
	{
		IEnvironmentProvider environmentProvider = new InMemoryEnvironmentProvider();
		
		throw new NotImplementedException();
	}
}
