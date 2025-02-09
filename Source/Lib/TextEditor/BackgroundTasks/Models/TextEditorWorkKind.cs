namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public enum TextEditorWorkKind
{
	RedundantTextEditorWork,
    UniqueTextEditorWork,
	OnDoubleClick,
    OnKeyDownLateBatching,
	OnMouseDown,
    OnMouseMove,
    OnScrollHorizontal,
	OnScrollVertical,
	OnWheel,
	OnWheelBatch
}
