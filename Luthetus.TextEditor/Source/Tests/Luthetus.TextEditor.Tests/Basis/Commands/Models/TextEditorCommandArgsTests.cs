using Xunit;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using System.Collections.Immutable;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models;

/// <summary>
/// <see cref="TextEditorCommandArgs"/>
/// </summary>
public class TextEditorCommandArgsTests
{
	/// <summary>
	/// <see cref="TextEditorCommandArgs(TextEditorModel, ImmutableArray{TextEditorCursorSnapshot}, bool, IClipboardService, ITextEditorService, TextEditorViewModel, Func{MouseEventArgs, Task}?, IJSRuntime?, Action{ResourceUri}?, Action{ResourceUri}?, Action{Key{TextEditorViewModel}}?)"/>
	/// </summary>
	[Fact]
	public void Constructor()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.Model"/>
	/// </summary>
	[Fact]
	public void Model()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.PrimaryCursor"/>
	/// </summary>
	[Fact]
	public void PrimaryCursorSnapshot()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.CursorBag"/>
	/// </summary>
	[Fact]
    public void CursorSnapshotsBag()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.ClipboardService"/>
	/// </summary>
	[Fact]
    public void ClipboardService()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.TextEditorService"/>
	/// </summary>
	[Fact]
    public void TextEditorService()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.ViewModel"/>
	/// </summary>
	[Fact]
    public void ViewModel()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.HandleMouseStoppedMovingEventAsyncFunc"/>
	/// </summary>
	[Fact]
    public void HandleMouseStoppedMovingEventAsyncFunc()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.JsRuntime"/>
	/// </summary>
	[Fact]
    public void JsRuntime()
    {
		//public IJSRuntime? JsRuntime { get; }
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.HasTextSelection"/>
	/// </summary>
	[Fact]
    public void HasTextSelection()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.RegisterModelAction"/>
	/// </summary>
	[Fact]
    public void RegisterModelAction()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.RegisterViewModelAction"/>
	/// </summary>
	[Fact]
    public void RegisterViewModelAction()
    {
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorCommandArgs.ShowViewModelAction"/>
	/// </summary>
	[Fact]
	public void ShowViewModelAction()
	{
		throw new NotImplementedException();
	}
}