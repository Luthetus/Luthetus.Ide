using Fluxor;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.States;

/// <summary>
/// Keep the <see cref="TextEditorDiffState"/> as a class
/// as to avoid record value comparisons when Fluxor checks
/// if the <see cref="FeatureStateAttribute"/> has been replaced.
/// </summary>
[FeatureState]
public partial class TextEditorDiffStateTests
{
    private TextEditorDiffState()
    {
        DiffModelBag = ImmutableList<TextEditorDiffModel>.Empty;
    }

    public ImmutableList<TextEditorDiffModel> DiffModelBag { get; init; }
}