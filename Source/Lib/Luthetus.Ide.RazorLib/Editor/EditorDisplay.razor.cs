using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Dimensions;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Store.EditorCase;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.HelperComponents;
using Luthetus.TextEditor.RazorLib.Semantics;
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

    private void HandleGotoDefinitionWithinDifferentFileAction(TextEditorSymbolDefinition textEditorSymbolDefinition)
    {
        var model = TextEditorService.Model.FindOrDefaultByResourceUri(
            textEditorSymbolDefinition.ResourceUri);

        if (model is null)
            return;

        var viewModels = TextEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

        if (!viewModels.Any())
        {
            Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
                new AbsoluteFilePath(model.ResourceUri.Value, false, EnvironmentProvider),
                true,
                EditorState.EditorTextEditorGroupKey));

            // TODO: Do not hackily create a ViewModel, and get a reference to it here
            viewModels = TextEditorService.Model.GetViewModelsOrEmpty(model.ModelKey);

            if (!viewModels.Any())
                return;
        }

        var viewModel = viewModels[0];

        var rowInformation = model.FindRowInformation(
            textEditorSymbolDefinition.PositionIndex);

        viewModel.PrimaryCursor.IndexCoordinates =
            (rowInformation.rowIndex,
                textEditorSymbolDefinition.PositionIndex - rowInformation.rowStartPositionIndex);

        Dispatcher.Dispatch(new EditorState.OpenInEditorAction(
            new AbsoluteFilePath(
                textEditorSymbolDefinition.ResourceUri.Value,
                false,
                EnvironmentProvider),
            true,
            EditorState.EditorTextEditorGroupKey));
    }
}