using CliWrap;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System.Reactive.Linq;
using System.Text;

namespace Luthetus.Ide.Tests.Basis.Terminals.Models;

public class CliWrapIntegratedTerminalTests
{
    [Fact]
    public void Constructor()
    {
        //public CliWrapIntegratedTerminal(string initialWorkingDirectory, IEnvironmentProvider environmentProvider)
        //    : base(initialWorkingDirectory, environmentProvider)
    }

    [Fact]
    public void StdInPipeSource()
    {
        //public PipeSource?  { get; private set; }
    }

    [Fact]
    public void AddStdOut()
    {
        //public void (string content, StdOutKind stdOutKind)
    }

    [Fact]
    public void AddStdInRequest()
    {
        //public void ()
    }

    [Fact]
    public void StartAsync()
    {
        //public override async Task (CancellationToken cancellationToken = default)
    }

    [Fact]
    public void GetRenderTreeBuilder()
    {
        //public override RenderTreeBuilder (RenderTreeBuilder builder, ref int sequence)
    }

    [Fact]
    public void StopAsync()
    {
        //public override Task (CancellationToken cancellationToken = default)
    }

    [Fact]
    public void HandleStdInputOnKeyDown()
    {
        //public override Task (
        //    KeyboardEventArgs keyboardEventArgs,
        //    StdInRequest stdInRequest,
        //    string capturedValue)
    }

    [Fact]
    public void HandleStdQuiescentOnKeyDown()
    {
        //public override Task (
        //    KeyboardEventArgs keyboardEventArgs,
        //    StdQuiescent stdQuiescent,
        //    string capturedTargetFilePath,
        //    string capturedArguments)
    }
}
