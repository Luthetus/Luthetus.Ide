using CliWrap;
using CliWrap.EventStream;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Ide.RazorLib.States.Models;
using Luthetus.Ide.RazorLib.Terminals.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public class TerminalSessionTests
{
    [Fact]
    public void Constructor()
    {
        //public TerminalSession(
        //    string? workingDirectoryAbsolutePathString,
        //    IDispatcher dispatcher,
        //    IBackgroundTaskService backgroundTaskService,
        //    ILuthetusCommonComponentRenderers commonComponentRenderers)
    }

    [Fact]
    public void TerminalSessionKey()
    {
        //public Key<TerminalSession>  { get; init; } = Key<TerminalSession>.NewKey();
    }

    [Fact]
    public void WorkingDirectoryAbsolutePathString()
    {
        //public string?  { get; private set; }
    }

    [Fact]
    public void ActiveTerminalCommand()
    {
        //public TerminalCommand?  { get; private set; }
    }

    [Fact]
    public void HasExecutingProcess()
    {
        //public bool  { get; private set; }
    }

    [Fact]
    public void TerminalCommandsHistory()
    {
        //public ImmutableArray<TerminalCommand>  => _terminalCommandsHistory.ToImmutableArray();
    }

    [Fact]
    public void ResourceUri()
    {
        //public ResourceUri  => new($"__LUTHETUS_{TerminalSessionKey.Guid}__");
    }

    [Fact]
    public void TextEditorViewModelKey()
    {
        //public Key<TextEditorViewModel>  => new(TerminalSessionKey.Guid);
    }

    [Fact]
    public void ReadStandardOut()
    {
        //public string ()
    }

    [Fact]
    public void ReadStandardOut()
    {
        //public string? (Key<TerminalCommand> terminalCommandKey)
    }

    [Fact]
    public void GetStandardOut()
    {
        //public List<string>? ()
    }

    [Fact]
    public void GetStandardOut()
    {
        //public List<string>? (Key<TerminalCommand> terminalCommandKey)
    }

    [Fact]
    public void EnqueueCommandAsync()
    {
        //public Task (TerminalCommand terminalCommand)
    }

    [Fact]
    public void ClearStandardOut()
    {
        //public void ()
    }

    [Fact]
    public void ClearStandardOut()
    {
        //public void (Key<TerminalCommand> terminalCommandKey)
    }

    [Fact]
    public void KillProcess()
    {
        //public void ()
    }
}