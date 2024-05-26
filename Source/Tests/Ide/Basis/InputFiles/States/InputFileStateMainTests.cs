using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.States;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

/// <summary>
/// <see cref="InputFileState"/>
/// </summary>
public class InputFileStateMainTests
{
    /// <summary>
    /// <see cref="InputFileState(int, ImmutableList{TreeViewAbsolutePath}, TreeViewAbsolutePath?, Func{IAbsolutePath?, Task}, Func{IAbsolutePath?, Task{bool}}, ImmutableArray{InputFilePattern}, InputFilePattern?, string, string)"/>
    /// <br/>----<br/>
    /// <see cref="InputFileState.IndexInHistory"/>
    /// <see cref="InputFileState.OpenedTreeViewModelHistoryList"/>
    /// <see cref="InputFileState.SelectedTreeViewModel"/>
    /// <see cref="InputFileState.OnAfterSubmitFunc"/>
    /// <see cref="InputFileState.SelectionIsValidFunc"/>
    /// <see cref="InputFileState.InputFilePatternsList"/>
    /// <see cref="InputFileState.SelectedInputFilePattern"/>
    /// <see cref="InputFileState.SearchQuery"/>
    /// <see cref="InputFileState.Message"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        throw new NotImplementedException();
    }
}