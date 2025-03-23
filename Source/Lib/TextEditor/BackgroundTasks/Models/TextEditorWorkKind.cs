namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public enum TextEditorWorkKind
{
	RedundantTextEditorWork,
    UniqueTextEditorWork,
	OnDoubleClick,
    OnKeyDown,
	OnMouseDown,
    OnMouseMove,
    OnScrollHorizontal,
	OnScrollVertical,
	OnWheel,
	OnWheelBatch
}
