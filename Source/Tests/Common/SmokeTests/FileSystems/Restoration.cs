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
	[InlineData("1 * DirectorySeparatorChar", isDirectory: false)]
	[InlineData("1 * DirectorySeparatorChar" isDirectory: true)]
	[InlineData("1 * AltDirectorySeparatorChar", false)]
	[InlineData("1 * AltDirectorySeparatorChar", true)]
	[InlineData("2 * DirectorySeparatorChar", isDirectory: false)]
	[InlineData("2 * DirectorySeparatorChar", isDirectory: true)]
	[InlineData("2 * AltDirectorySeparatorChar", isDirectory: false)]
	[InlineData("2 * AltDirectorySeparatorChar", isDirectory: true)]
	[InlineData("3 * DirectorySeparatorChar", isDirectory: false)]
	[InlineData("3 * DirectorySeparatorChar", isDirectory: true)]
	[InlineData("3 * AltDirectorySeparatorChar", isDirectory: false)]
	[InlineData("3 * AltDirectorySeparatorChar", isDirectory: true)]
	// - (1, 2, 3) * "C:"
	[InlineData(1 * "C:", isDirectory: false)]
	[InlineData(1 * "C:", isDirectory: true)]
	[InlineData(2 * "C:", isDirectory: false)]
	[InlineData(2 * "C:", isDirectory: true)]
	[InlineData(3 * "C:", isDirectory: false)]
	[InlineData(3 * "C:", isDirectory: true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar)
	[InlineData("C:" + DirectorySeparatorChar, isDirectory: false)]
	[InlineData("C:" + DirectorySeparatorChar, isDirectory: true)]
	[InlineData("C:" + AltDirectorySeparatorChar, isDirectory: false)]
	[InlineData("C:" + AltDirectorySeparatorChar, isDirectory: true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:"
	[InlineData(DirectorySeparatorChar + "C:", isDirectory: false)]
	[InlineData(DirectorySeparatorChar + "C:", isDirectory: true)]
	[InlineData(AltDirectorySeparatorChar + "C:", isDirectory: false)]
	[InlineData(AltDirectorySeparatorChar + "C:", isDirectory: true)]
	// - "C:abc.txt"
	[InlineData("C:abc.txt", isDirectory: false)]
	[InlineData("C:abc.txt", isDirectory: true)]
	// - "C:" + (DirectorySeparatorChar/AltDirectorySeparatorChar) + "abc.txt"
	[InlineData("C:" + DirectorySeparatorChar + "abc.txt", isDirectory: false)]
	[InlineData("C:" + DirectorySeparatorChar + "abc.txt", isDirectory: true)]
	[InlineData("C:" + AltDirectorySeparatorChar + "abc.txt", isDirectory: false)]
	[InlineData("C:" + AltDirectorySeparatorChar + "abc.txt", isDirectory: true)]
	// - (DirectorySeparatorChar/AltDirectorySeparatorChar) + "C:" + "abc.txt"
	[InlineData(DirectorySeparatorChar + "C:" + "abc.txt", isDirectory: false)]
	[InlineData(DirectorySeparatorChar + "C:" + "abc.txt", isDirectory: true)]
	[InlineData(AltDirectorySeparatorChar + "C:" + "abc.txt", isDirectory: false)]
	[InlineData(AltDirectorySeparatorChar + "C:" + "abc.txt", isDirectory: true)]
    
	public void Root_Directory()
	{
		IEnvironmentProvider environmentProvider = new InMemoryEnvironmentProvider();
		
		string absolutePathString;
		bool isDirectory;
		
		
		
		
    		
		throw new NotImplementedException();
	}
}
