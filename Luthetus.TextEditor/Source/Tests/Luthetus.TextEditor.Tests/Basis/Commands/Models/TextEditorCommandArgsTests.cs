using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

public class TextEditorCommandArgsTests
{
	[Fact]
	public void Constructor()
	{
	    //public TextEditorCommandArgs(
     //       TextEditorModel textEditor,
     //       ImmutableArray<TextEditorCursorSnapshot> cursorSnapshotsBag,
     //       bool hasTextSelection,
     //       IClipboardService clipboardService,
     //       ITextEditorService textEditorService,
     //       TextEditorViewModel textEditorViewModel,
     //       Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
     //       IJSRuntime? jsRuntime,
     //       Action<ResourceUri>? registerModelAction,
     //       Action<ResourceUri>? registerViewModelAction,
     //       Action<Key<TextEditorViewModel>>? showViewModelAction)
	}

	[Fact]
	public void Model()
	{
	    //public TextEditorModel Model { get; }
	}

	[Fact]
	public void PrimaryCursorSnapshot()
	{
	    //public TextEditorCursorSnapshot PrimaryCursorSnapshot
	}

    [Fact]
    public void CursorSnapshotsBag()
    {
        //public ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag
    }

    [Fact]
    public void ClipboardService()
    {
        //public IClipboardService ClipboardService
    }

    [Fact]
    public void TextEditorService()
    {
        //public ITextEditorService TextEditorService { get; }
    }

    [Fact]
    public void ViewModel()
    {
        //public TextEditorViewModel ViewModel { get; }
    }

    [Fact]
    public void HandleMouseStoppedMovingEventAsyncFunc()
    {
        //public Func<MouseEventArgs, Task>? HandleMouseStoppedMovingEventAsyncFunc { get; }
    }

    [Fact]
    public void JsRuntime()
    {
        //public IJSRuntime? JsRuntime { get; }
    }

    [Fact]
    public void HasTextSelection()
    {
        //public bool HasTextSelection { get; set; }
    }

    [Fact]
    public void RegisterModelAction()
    {
        //public Action<ResourceUri>? RegisterModelAction { get; set; }
    }

    [Fact]
    public void RegisterViewModelAction()
    {
        //public Action<ResourceUri>? RegisterViewModelAction { get; set; }
    }

	[Fact]
	public void ShowViewModelAction()
	{
	    //public Action<Key<TextEditorViewModel>>? ShowViewModelAction { get; set; }
	}
}