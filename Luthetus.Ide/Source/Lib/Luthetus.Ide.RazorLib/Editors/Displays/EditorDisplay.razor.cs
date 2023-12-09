using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Editors.Displays;

public partial class EditorDisplay : ComponentBase
{
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;

    [Parameter, EditorRequired]
    public ElementDimensions EditorElementDimensions { get; set; } = null!;

    private static readonly ImmutableArray<TextEditorHeaderButtonKind> TextEditorHeaderButtonKindsBag =
        Enum.GetValues(typeof(TextEditorHeaderButtonKind))
            .Cast<TextEditorHeaderButtonKind>()
            .ToImmutableArray();

    private TextEditorViewModelDisplayOptions _viewModelDisplayOptions = null!;

    protected override void OnInitialized()
    {
        _viewModelDisplayOptions = new()
        {
            WrapperClassCssString = "luth_te_demo-text-editor",
            TabIndex = 0,
            HeaderButtonKinds = TextEditorHeaderButtonKindsBag,
            RegisterModelAction = RegisterModelAction,
            ShowViewModelAction = ShowViewModelAction,
        };

        base.OnInitialized();
    }

    private void RegisterModelAction(ResourceUri resourceUri)
    {
        var absolutePath = new AbsolutePath(resourceUri.Value, false, EnvironmentProvider);
        EditorSync.OpenInEditor(absolutePath, true);
    }

    private void ShowViewModelAction(Key<TextEditorViewModel> viewModelKey)
    {
        TextEditorService.Group.SetActiveViewModel(EditorSync.EditorTextEditorGroupKey, viewModelKey);
    }
}