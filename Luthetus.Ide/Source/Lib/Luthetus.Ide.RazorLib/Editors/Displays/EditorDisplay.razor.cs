using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Editors.Displays;

public partial class EditorDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

    private static readonly ImmutableArray<HeaderButtonKind> TextEditorHeaderButtonKindsList =
        Enum.GetValues(typeof(HeaderButtonKind))
            .Cast<HeaderButtonKind>()
            .ToImmutableArray();

    private ViewModelDisplayOptions _viewModelDisplayOptions = null!;

    protected override void OnInitialized()
    {
        _viewModelDisplayOptions = new()
        {
            WrapperClassCssString = "luth_te_demo-text-editor",
            TabIndex = 0,
            HeaderButtonKinds = TextEditorHeaderButtonKindsList,
        };

        base.OnInitialized();
    }
}