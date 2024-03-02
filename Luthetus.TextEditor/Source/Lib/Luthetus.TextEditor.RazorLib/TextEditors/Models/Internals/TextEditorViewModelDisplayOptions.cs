using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public class TextEditorViewModelDisplayOptions
{
    public string WrapperStyleCssString { get; set; } = string.Empty;
    public string WrapperClassCssString { get; set; } = string.Empty;
    public string TextEditorStyleCssString { get; set; } = string.Empty;
    public string TextEditorClassCssString { get; set; } = string.Empty;
    /// <summary>
    /// TabIndex is used for the html attribute named: 'tabindex'
    /// </summary>
    public int TabIndex { get; set; } = -1;
    public RenderFragment? ContextMenuRenderFragmentOverride { get; set; }
    public RenderFragment? AutoCompleteMenuRenderFragmentOverride { get; set; }
    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownAsync"/> will be used.
    /// </summary>
    public Func<
        ResourceUri,
        Key<TextEditorViewModel>,
        KeyboardEventArgs,
        Func<TextEditorMenuKind, bool, Task>,
        TextEditorEdit>?
        AfterOnKeyDownAsyncFactory { get; set; }
    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownRangeAsync"/> will be used.
    /// 
    /// If a batch handling of KeyboardEventArgs is performed, then this method
    /// will be invoked as opposed to <see cref="AfterOnKeyDownAsyncFactory"/>,
    /// and a list of <see cref="KeyboardEventArgs"/> will be provided,
    /// sorted such that the first index represents the first event fired,
    /// and the last index represents the last event fired.
    /// </summary>
    public Func<
        ResourceUri,
        Key<TextEditorViewModel>,
        List<KeyboardEventArgs>,
        Func<TextEditorMenuKind, bool, Task>,
        TextEditorEdit>?
        AfterOnKeyDownRangeAsyncFactory { get; set; }
    /// <summary>
    /// If set to false the <see cref="TextEditorHeader"/> will NOT render above the text editor.
    /// </summary>
    public bool IncludeHeaderHelperComponent { get; set; } = true;
    /// <summary>
    /// <see cref="HeaderButtonKinds"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.
    /// </summary>
    public ImmutableArray<TextEditorHeaderButtonKind>? HeaderButtonKinds { get; set; }
    /// <summary>
    /// If set to false the <see cref="TextEditorFooter"/> will NOT render below the text editor.
    /// </summary>
    public bool IncludeFooterHelperComponent { get; set; } = true;
    public bool IncludeContextMenuHelperComponent { get; set; } = true;
}
