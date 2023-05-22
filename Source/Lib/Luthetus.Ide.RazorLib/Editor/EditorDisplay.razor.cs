using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.TextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

    private static readonly ImmutableArray<TextEditorHeaderButtonKind> TextEditorHeaderButtonKinds =
        Enum
            .GetValues(typeof(TextEditorHeaderButtonKind))
            .Cast<TextEditorHeaderButtonKind>()
            .ToImmutableArray();
}