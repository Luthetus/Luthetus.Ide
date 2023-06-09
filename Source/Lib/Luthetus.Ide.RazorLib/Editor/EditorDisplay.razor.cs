using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.HelperComponents;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.Editor;

public partial class EditorDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

    private static readonly ImmutableArray<TextEditorHeaderButtonKind> TextEditorHeaderButtonKinds =
        Enum
            .GetValues(typeof(TextEditorHeaderButtonKind))
            .Cast<TextEditorHeaderButtonKind>()
            .ToImmutableArray();
}