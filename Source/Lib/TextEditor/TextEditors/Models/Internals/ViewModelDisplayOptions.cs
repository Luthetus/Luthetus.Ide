using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public sealed class ViewModelDisplayOptions
{
    public string TextEditorStyleCssString { get; set; } = string.Empty;
    public string TextEditorClassCssString { get; set; } = string.Empty;
    
    /// <summary>
    /// TabIndex is used for the html attribute named: 'tabindex'
    /// </summary>
    public int TabIndex { get; set; } = -1;
    public RenderFragment<TextEditorRenderBatch>? ContextMenuRenderFragmentOverride { get; set; }
    public RenderFragment<TextEditorRenderBatch>? AutoCompleteMenuRenderFragmentOverride { get; set; }

    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownAsync"/> will be used.
    /// </summary>
    public Func<
    	TextEditorEditContext,
        TextEditorModel,
        TextEditorViewModel,
        CursorModifierBagTextEditor,
        KeymapArgs,
		TextEditorComponentData,
        Task>?
        AfterOnKeyDownAsync { get; set; }

    /// <summary>
    /// If left null, the default <see cref="HandleAfterOnKeyDownRangeAsync"/> will be used.
    /// 
    /// If a batch handling of KeyboardEventArgs is performed, then this method will be invoked as opposed to
    /// <see cref="AfterOnKeyDownAsyncFactory"/>, and a list of <see cref="KeyboardEventArgs"/> will be provided,
    /// sorted such that the first index represents the first event fired, and the last index represents the last
    /// event fired.
    /// </summary>
    public Func<
    	TextEditorEditContext,
        TextEditorModel,
        TextEditorViewModel,
        CursorModifierBagTextEditor,
        KeymapArgs[], // batchKeymapArgsList
        int, // batchKeymapArgsListLength
        TextEditorComponentData,
        Task>?
        AfterOnKeyDownRangeAsync { get; set; }

    /// <summary>
    /// If set to null then no header will render.
    ///
    /// If overriden to non-null, the component is expected to have
    /// CSS style attribute height="var(--luth_te_text-editor-header-height)"
    /// </summary>
    public Type? HeaderComponentType { get; set; } = typeof(TextEditorDefaultHeaderDisplay);

    /// <summary>
    /// <see cref="HeaderButtonKinds"/> contains the enum value that represents a button displayed in the optional component: <see cref="TextEditorHeader"/>.
    /// </summary>
    public List<HeaderButtonKind>? HeaderButtonKinds { get; set; }

    /// <summary>
    /// If set to null then no footer will render.
    ///
    /// If overriden to non-null, the component is expected to have
    /// CSS style attribute height="var(--luth_te_text-editor-footer-height)"
    /// </summary>
    public Type? FooterComponentType { get; set; } = typeof(TextEditorDefaultFooterDisplay);

    /// <summary>
    /// If set to false: the <see cref="Displays.Internals.GutterSection"/> will NOT render. (i.e. line numbers will not render)
    /// </summary>
    public bool IncludeGutterComponent { get; set; } = true;

    public bool IncludeContextMenuHelperComponent { get; set; } = true;

    public ContextRecord ContextRecord { get; set; } = ContextFacts.TextEditorContext;

	/// <summary>
	/// The integrated terminal logic needs a keymap, separate to that of the 'global' keymap used by other text editors.
	/// Therefore, this property is used to provide the <see cref="Keymaps.Models.Terminals.TextEditorKeymapTerminal"/>
	/// to the integrated terminal.<br/><br/>
	/// 
	/// This property is not intended for use in any other scenario.
	/// </summary>
	public Keymap? KeymapOverride { get; set; }
}
