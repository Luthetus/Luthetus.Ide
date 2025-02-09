namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public enum TextEditorWorkKind
{
	OnDoubleClick,
    OnKeyDownLateBatching,
	OnMouseDown,
    OnMouseMove,
    OnScrollHorizontal,
	OnScrollVertical,
	OnWheel,
	OnWheelBatch
}
