using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;
// This is an editor just the same as the 'Main' editor.
// Single click on the entry in the search results opens
// this in-dialog-editor.
//
// These editors are synced.
// Editing one or the other will
// update the other.
//
// Here I am editing from the
// main editor.
//
// { Ctrl + s } keybind lets me save a file.
// This keybind works in the dialog-editor
// and the main editor.
//
// A double click will open the file in the 'Main' editor.
public record TerminalCommand(
    Key<TerminalCommand> TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null, // Here is the ContinueWith Func
	Func<Task>? BeginWith = null);