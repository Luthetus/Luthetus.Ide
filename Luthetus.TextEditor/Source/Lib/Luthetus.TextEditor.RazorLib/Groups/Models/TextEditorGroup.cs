using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Groups.Models;

/// <summary>Store the state of none or many tabs, and which tab is the active one. Each tab represents a <see cref="TextEditorViewModel"/>.</summary>
public record TextEditorGroup(
    Key<TextEditorGroup> GroupKey,
    Key<TextEditorViewModel> ActiveViewModelKey,
    ImmutableList<Key<TextEditorViewModel>> ViewModelKeyList)
{
    public Key<RenderState> RenderStateKey { get; init; } = Key<RenderState>.NewKey();
}