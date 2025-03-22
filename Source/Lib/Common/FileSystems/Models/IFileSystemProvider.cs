namespace Luthetus.Common.RazorLib.FileSystems.Models;

public interface IFileSystemProvider
{
    public IFileHandler File { get; }
    public IDirectoryHandler Directory { get; }
    
    /// <summary>
    /// TODO: I don't have Mac / Linux available at the moment...
    /// ...Do all the OS return the path
    /// without a directory separator character at the end?
    ///
    /// TODO: If not all OS return without a directory separator character at the end,...
    /// ...then this should have 3 cases:
    /// - Directory separator at the end
    /// - AltDirectory separator at the end
    /// - No directory separator at the end
    /// </summary>
	public static bool IsDirectoryIgnored(string directory)
	{
		if (directory.EndsWith("\\"))
		{
			if (directory.EndsWith(".git\\") ||
				directory.EndsWith(".vs\\") ||
				directory.EndsWith(".vscode\\") ||
				directory.EndsWith(".idea\\") ||
				directory.EndsWith("bin\\") ||
				directory.EndsWith("obj\\"))
			{
				return true;
			}
		}
		else if (directory.EndsWith("/"))
		{
			if (directory.EndsWith(".git/") ||
				directory.EndsWith(".vs/") ||
				directory.EndsWith(".vscode/") ||
				directory.EndsWith(".idea/") ||
				directory.EndsWith("bin/") ||
				directory.EndsWith("obj/"))
			{
				return true;
			}
		}
		else
		{
			if (directory.EndsWith(".git") ||
				directory.EndsWith(".vs") ||
				directory.EndsWith(".vscode") ||
				directory.EndsWith(".idea") ||
				directory.EndsWith("bin") ||
				directory.EndsWith("obj"))
			{
				return true;
			}
		}
		
		return false;
	}
}