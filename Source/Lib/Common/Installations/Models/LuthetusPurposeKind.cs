namespace Luthetus.Common.RazorLib.Installations.Models;

/// <summary>
/// Common and TextEditor only require 1 background thread.
/// Whereas the Ide requires 2 background threads.
///
/// To use Common or TextEditor registers 1 background thread.
/// To use Ide registers 2 background threads.
///
/// Common and TextEditor at the moment are the same,
/// but both enum members were made to avoid confusion
/// if anyone references only Luthetus.Common csproj
/// </summary>
public enum LuthetusPurposeKind
{
	Common,
	TextEditor,
	Ide,
}
