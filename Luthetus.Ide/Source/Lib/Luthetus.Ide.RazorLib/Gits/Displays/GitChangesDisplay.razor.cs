using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitChangesDisplay : ComponentBase, IGitDisplayRendererType
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;

    private static readonly Key<TextEditorDiffModel> DiffModelKey = Key<TextEditorDiffModel>.NewKey();

    private static readonly ResourceUri BeforeResourceUri = new(nameof(GitChangesDisplay) + "_in");
    private static readonly ResourceUri AfterResourceUri = new(nameof(GitChangesDisplay) + "_out");

    private static readonly Key<TextEditorViewModel> BeforeViewModelKey = Key<TextEditorViewModel>.NewKey();
    private static readonly Key<TextEditorViewModel> AfterViewModelKey = Key<TextEditorViewModel>.NewKey();

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // "In" Registrations
            {
                TextEditorService.Model.RegisterTemplated(
                    DecorationMapperRegistry,
                    CompilerServiceRegistry,
                    ExtensionNoPeriodFacts.TXT,
                    BeforeResourceUri,
                    DateTime.UtcNow,
                    "ABCDEFK",
                    "Before");

                TextEditorService.Model.RegisterPresentationModel(
                    BeforeResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);
                
                TextEditorService.Model.RegisterPresentationModel(
                    BeforeResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModel.Register(
                    BeforeViewModelKey,
                    BeforeResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.InPresentationKey,
                };

                TextEditorService.ViewModel.With(
                    BeforeViewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                    });
            }
            
            // "Out" Registrations
            {
                TextEditorService.Model.RegisterTemplated(
                    DecorationMapperRegistry,
                    CompilerServiceRegistry,
                    ExtensionNoPeriodFacts.TXT,
                    AfterResourceUri,
                    DateTime.UtcNow,
                    "BHDEFCK",
                    "After");

                TextEditorService.Model.RegisterPresentationModel(
                    AfterResourceUri,
                    DiffPresentationFacts.EmptyInPresentationModel);

                TextEditorService.Model.RegisterPresentationModel(
                    AfterResourceUri,
                    DiffPresentationFacts.EmptyOutPresentationModel);

                TextEditorService.ViewModel.Register(
                    AfterViewModelKey,
                    AfterResourceUri);

                var presentationKeys = new[]
                {
                    DiffPresentationFacts.OutPresentationKey,
                };

                TextEditorService.ViewModel.With(
                    AfterViewModelKey,
                    textEditorViewModel => textEditorViewModel with
                    {
                        FirstPresentationLayerKeysBag = presentationKeys.ToImmutableList()
                    });
            }

            TextEditorService.Diff.Register(
                DiffModelKey,
                BeforeViewModelKey,
                AfterViewModelKey);
        }

        return base.OnAfterRenderAsync(firstRender);
    }
}