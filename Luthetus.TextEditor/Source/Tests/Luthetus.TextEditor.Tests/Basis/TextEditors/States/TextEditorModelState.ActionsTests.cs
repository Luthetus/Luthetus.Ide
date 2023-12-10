using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

public class TextEditorModelStateActionsTests
{
	[Fact]
	public void RegisterAction()
	{
		//public record RegisterAction(TextEditorModel Model);
		throw new NotImplementedException();
	}

	[Fact]
	public void DisposeAction()
	{
		//public record DisposeAction(ResourceUri ResourceUri);
		throw new NotImplementedException();
	}

	[Fact]
	public void UndoEditAction()
	{
		//public record UndoEditAction(ResourceUri ResourceUri);
		throw new NotImplementedException();
	}

	[Fact]
	public void RedoEditAction()
	{
		//public record RedoEditAction(ResourceUri ResourceUri);
		throw new NotImplementedException();
	}

	[Fact]
	public void ForceRerenderAction()
	{
		//public record ForceRerenderAction(ResourceUri ResourceUri);
		throw new NotImplementedException();
	}

	[Fact]
	public void RegisterPresentationModelAction()
	{
		//public record RegisterPresentationModelAction(ResourceUri ResourceUri, TextEditorPresentationModel PresentationModel);
		throw new NotImplementedException();
	}

	[Fact]
	public void CalculatePresentationModelAction()
	{
		//public record CalculatePresentationModelAction(ResourceUri ResourceUri, Key<TextEditorPresentationModel> PresentationKey);
		throw new NotImplementedException();
	}

	[Fact]
	public void ReloadAction()
	{
		//public record ReloadAction(ResourceUri ResourceUri, string Content, DateTime ResourceLastWriteTime);
		throw new NotImplementedException();
	}

	[Fact]
	public void SetResourceDataAction()
	{
		//public record SetResourceDataAction(ResourceUri ResourceUri, DateTime ResourceLastWriteTime);
		throw new NotImplementedException();
	}

	[Fact]
	public void SetUsingRowEndingKindAction()
	{
		//public record SetUsingRowEndingKindAction(ResourceUri ResourceUri, RowEndingKind RowEndingKind);
		throw new NotImplementedException();
	}

	[Fact]
	public void KeyboardEventAction()
	{
		//public record KeyboardEventAction(
	 //       ResourceUri ResourceUri,
	 //       ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
	 //       KeyboardEventArgs KeyboardEventArgs,
	 //       CancellationToken CancellationToken);
		throw new NotImplementedException();
	}

	[Fact]
	public void InsertTextAction()
	{
		//public record InsertTextAction(
	 //       ResourceUri ResourceUri,
	 //       ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
	 //       string Content,
	 //       CancellationToken CancellationToken);
		throw new NotImplementedException();
	}

	[Fact]
	public void DeleteTextByMotionAction()
	{
		//public record DeleteTextByMotionAction(
	 //       ResourceUri ResourceUri,
	 //       ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
	 //       MotionKind MotionKind,
	 //       CancellationToken CancellationToken);
		throw new NotImplementedException();
	}

	[Fact]
	public void DeleteTextByRangeAction()
	{
		//public record DeleteTextByRangeAction(
	 //       ResourceUri ResourceUri,
	 //       ImmutableArray<TextEditorCursorSnapshot> CursorSnapshotsBag,
	 //       int Count,
	 //       CancellationToken CancellationToken);
		throw new NotImplementedException();
	}
}