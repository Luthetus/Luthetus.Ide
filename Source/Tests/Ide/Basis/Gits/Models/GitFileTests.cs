using Luthetus.Ide.RazorLib.Gits.Models;

namespace Luthetus.Ide.Tests.Basis.Gits.Models;

/// <summary>
/// <see cref="GitFile"/>
/// </summary>
public class GitFileTests
{
    /// <summary>
    /// <see cref="GitFile.AbsolutePath"/>
    /// </summary>
    [Fact]
    public void AbsolutePath()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="GitFile.GitDirtyReason"/>
    /// </summary>
    [Fact]
    public void GitDirtyReason()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="GitFile.IsDirty"/>
    /// </summary>
    [Fact]
    public void IsDirty()
    {
        throw new NotImplementedException();
    }

	[Fact]
	public void UnixFileSystemShouldParseProperly()
	{
		/*
/home/hunter/Repos/Luthetus.Ide_Fork/>git status -u
On branch progress
Your branch is up to date with 'origin/progress'.

Changes not staged for commit:
  (use "git add <file>..." to update what will be committed)
  (use "git restore <file>..." to discard changes in working directory)
	modified:   Source/Lib/Ide/Ide.RazorLib/Gits/Displays/TreeViewGitFileDisplay.razor

Untracked files:
  (use "git add <file>..." to include in what will be committed)
	Source/Lib/Ide/Ide.RazorLib/Gits/Displays/TreeViewGitFileDisplay.razor.css

no changes added to commit (use "git add" and/or "git commit -a")
Process exited; Code: 0
		*/

		// Example output is above this comment.
		// The parsing code works for Windows, but not Linux.
		//
		// Untracked file is output as:
		// "Source/Lib/Ide/Ide.RazorLib/Gits/Displays/TreeViewGitFileDisplay.razor.css"
		//
		// When parsed it erroneously is:
		// "Luthetus.Ide_Fork/Source/Lib/Ide/Ide.RazorLib/Gits/Displays/TreeViewGitFileDisplay.razor.css"
		//
		// Excepted parse result:
		// "Source/Lib/Ide/Ide.RazorLib/Gits/Displays/TreeViewGitFileDisplay.razor.css"

		// var gitCliParser = new GitCliOutputParser();
	}
}