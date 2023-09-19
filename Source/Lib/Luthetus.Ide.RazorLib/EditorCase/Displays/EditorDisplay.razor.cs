using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.TextEditor.RazorLib.TextEditorCase.Scenes.InternalClasses;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.EditorCase.Displays;

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