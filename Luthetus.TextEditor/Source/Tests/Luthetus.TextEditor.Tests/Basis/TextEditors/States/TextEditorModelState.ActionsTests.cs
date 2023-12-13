using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorModelState"/>
/// </summary>
public class TextEditorModelStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorModelState.RegisterAction"/>
	/// </summary>
	[Fact]
	public void RegisterAction()
	{
		// var registerAction = new RegisterAction();

		/*
	public record RegisterAction(TextEditorModel Model);
    public record DisposeAction(ResourceUri ResourceUri);
    public record UndoEditAction(ResourceUri ResourceUri);
    public record RedoEditAction(ResourceUri ResourceUri);
    public record ForceRerenderAction(ResourceUri ResourceUri);
    public record RegisterPresentationModelAction(ResourceUri ResourceUri, TextEditorPresentationModel PresentationModel);
    public record CalculatePresentationModelAction(ResourceUri ResourceUri, Key<TextEditorPresentationModel> PresentationKey);
    public record ReloadAction(ResourceUri ResourceUri, string Content, DateTime ResourceLastWriteTime);
    public record SetResourceDataAction(ResourceUri ResourceUri, DateTime ResourceLastWriteTime);
    public record SetUsingRowEndingKindAction(ResourceUri ResourceUri, RowEndingKind RowEndingKind);

    public record KeyboardEventAction(
        ResourceUri ResourceUri,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
        KeyboardEventArgs KeyboardEventArgs,
        CancellationToken CancellationToken);

    public record InsertTextAction(
        ResourceUri ResourceUri,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
        string Content,
        CancellationToken CancellationToken);

    public record DeleteTextByMotionAction(
        ResourceUri ResourceUri,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
        MotionKind MotionKind,
        CancellationToken CancellationToken);

    public record DeleteTextByRangeAction(
        ResourceUri ResourceUri,
        ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
        int Count,
        CancellationToken CancellationToken);
		*/

		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DisposeAction"/>
	/// </summary>
	[Fact]
	public void DisposeAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.UndoEditAction"/>
	/// </summary>
	[Fact]
	public void UndoEditAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.RedoEditAction"/>
	/// </summary>
	[Fact]
	public void RedoEditAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.ForceRerenderAction"/>
	/// </summary>
	[Fact]
	public void ForceRerenderAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.RegisterPresentationModelAction"/>
	/// </summary>
	[Fact]
	public void RegisterPresentationModelAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.CalculatePresentationModelAction"/>
	/// </summary>
	[Fact]
	public void CalculatePresentationModelAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.ReloadAction"/>
	/// </summary>
	[Fact]
	public void ReloadAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.SetResourceDataAction"/>
	/// </summary>
	[Fact]
	public void SetResourceDataAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.SetUsingRowEndingKindAction"/>
	/// </summary>
	[Fact]
	public void SetUsingRowEndingKindAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.KeyboardEventAction"/>
	/// </summary>
	[Fact]
	public void KeyboardEventAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.InsertTextAction"/>
	/// </summary>
	[Fact]
	public void InsertTextAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DeleteTextByMotionAction"/>
	/// </summary>
	[Fact]
	public void DeleteTextByMotionAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DeleteTextByRangeAction"/>
	/// </summary>
	[Fact]
	public void DeleteTextByRangeAction()
	{
		throw new NotImplementedException();
	}
}