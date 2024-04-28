using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

/// <summary>
/// <see cref="InputFileSync"/>
/// </summary>
public class InputFileSyncEnqueuesTests
{
    /// <summary>
    /// <see cref="InputFileSync.RequestInputFileStateForm(string, Func{IAbsolutePath?, Task}, Func{IAbsolutePath?, Task{bool}}, ImmutableArray{InputFilePattern})"/>
    /// </summary>
    [Fact]
    public void RequestInputFileStateForm()
    {
        throw new NotImplementedException();
    }
}