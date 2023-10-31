using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.Diffs.Displays.Internals;

public partial class DiffDetailsMatrixDisplay : ComponentBase
{
    [CascadingParameter]
    public TextEditorDiffResult DiffResult { get; set; } = null!;
}