using Fluxor;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.Tests.Basis.InputFiles.States;

public class InputFileStateMainTests
{
    [Fact]
    public void Constructor()
    {
        //private InputFileState() : this(
        //    -1,
        //    ImmutableList<TreeViewAbsolutePath>.Empty,
        //    null,
        //    _ => Task.CompletedTask,
        //    _ => Task.FromResult(false),
        //    ImmutableArray<InputFilePattern>.Empty,
        //    null,
        //    string.Empty,
        //    string.Empty)
        //{
        //}
    }

    [Fact]
    public void IndexInHistory()
    {
        //int ,
    }

    [Fact]
    public void OpenedTreeViewModelHistoryList()
    {
        //ImmutableList<TreeViewAbsolutePath> ,
    }

    [Fact]
    public void SelectedTreeViewModel()
    {
        //TreeViewAbsolutePath? ,
    }

    [Fact]
    public void OnAfterSubmitFunc()
    {
        //Func<IAbsolutePath?, Task> ,
    }

    [Fact]
    public void SelectionIsValidFunc()
    {
        //Func<IAbsolutePath?, Task<bool>> ,
    }

    [Fact]
    public void InputFilePatternsList()
    {
        //ImmutableArray<InputFilePattern> ,
    }

    [Fact]
    public void SelectedInputFilePattern()
    {
        //InputFilePattern? ,
    }

    [Fact]
    public void SearchQuery()
    {
        //string ,
    }

    [Fact]
    public void Message()
    {
        //string )
    }
}