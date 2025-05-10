using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorModelApi
{
    #region CREATE_METHODS
    /// <summary>It is recommended to use the <see cref="RegisterTemplated" /> method as it will internally reference the <see cref="ITextEditorLexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
    public void RegisterCustom(TextEditorEditContext editContext, TextEditorModel model);
    /// <summary>
    /// (2025-01-25)
    /// TODO: This is extremely confusing. The 'ICompilerService' implementations sometimes require...
    /// ...the 'ITextEditorService' itself.
    /// This causes a dependency "circular reference" or some such wording.
    /// |
    /// Thus, at some point the 'ITextEditorRegistryWrap' was made so that the
    /// 'ITextEditorService' could dependency inject a "wrapper" that contains
    /// the properties for 'ICompilerServiceRegistry' and 'IDecorationMapperRegistry'.
    /// |
    /// But, this is done via some hacky 'setters' on the properties.
    /// |
    /// You have to set the 'ITextEditorRegistryWrap'.CompilerServiceRegistry and
    /// 'ITextEditorRegistryWrap'.DecorationMapperRegistry after the service provider
    /// was built.
    /// |
    /// I "fixed" this by adding to 'LuthetusTextEditorInitializer' 'OnInitialized(...)'
    /// blazor lifecycle method the 'set' expressions.
    /// |
    /// All in all this is just an odd thing to do.
    /// ============================================================================================
    /// 
    /// Plain Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
    /// of "txt" or the constant varible: <see cref="ExtensionNoPeriodFacts.TXT"/>.<br /><br />
    /// 
    /// C# Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
    /// of "cs" or the constant varible: <see cref="ExtensionNoPeriodFacts.C_SHARP_CLASS"/>;
    /// NOTE: One must first install the Luthetus.CompilerServices.CSharp NuGet package.<br /><br />
    /// </summary>
    public void RegisterTemplated(
    	TextEditorEditContext editContext,
        string extensionNoPeriod,
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string initialContent,
        string? overrideDisplayTextForFileExtension = null);
    #endregion

    #region READ_METHODS
    /// <summary>
	/// Returns a shallow copy
	///
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public Dictionary<ResourceUri, TextEditorModel> GetModels();
    public int GetModelsCount();
    public TextEditorModel? GetOrDefault(ResourceUri resourceUri);
    public List<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
    public string? GetAllText(ResourceUri resourceUri);
    #endregion

    #region UPDATE_METHODS
    public void UndoEdit(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier);

    public void SetUsingLineEndKind(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        LineEndKind lineEndKind);

    public void SetResourceData(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        DateTime resourceLastWriteTime);

    public void Reload(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        string content,
        DateTime resourceLastWriteTime);

    public void RedoEdit(
    	TextEditorEditContext editContext,
        TextEditorModel modelModifier);

    public void InsertText(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="InsertTextFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public void InsertTextUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content);

    public void HandleKeyboardEvent(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="HandleKeyboardEvent"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public void HandleKeyboardEventUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeymapArgs keymapArgs);

    public void DeleteTextByRange(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByRangeFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public void DeleteTextByRangeUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count);

    public void DeleteTextByMotion(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="DeleteTextByMotionFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public void DeleteTextByMotionUnsafe(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind);

    public void AddPresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        TextEditorPresentationModel emptyPresentationModel);

    /// <param name="emptyPresentationModel">
    /// If the presentation model was not found, the empty presentation model will be registered.
    /// </param>
    public void StartPendingCalculatePresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel);

    /// <param name="emptyPresentationModel">
    /// If the presentation model was not found, the empty presentation model will be registered.
    /// </param>
    public void CompletePendingCalculatePresentationModel(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        List<TextEditorTextSpan> calculatedTextSpans);

    /// <summary>
    /// If applying syntax highlighting it may be preferred to use <see cref="ApplySyntaxHighlightingAsync" />.
    /// </summary>
    public void ApplyDecorationRange(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier,
        IEnumerable<TextEditorTextSpan> textSpans);

	/// <summary>
	/// This method is more performant than <see cref="ApplyDecorationRange"/>
	/// if the tokens, misc-text-spans, and symbols, are all stored in separate lists.
	///
	/// Because rather than combine them as an IEnumerable,
	/// this will iterate each list directly.
	///
	/// TODO: What is the exact overhead of using the IEnumerable approach...
	/// ...The reality is that those 3 sources were being combined into a new List
	/// previously. But I wonder if IEnumerable would have any meaningful overhead?
	/// </summary>
    public void ApplySyntaxHighlighting(
        TextEditorEditContext editContext,
        TextEditorModel modelModifier);
    #endregion

    #region DELETE_METHODS
    public void Dispose(TextEditorEditContext editContext, ResourceUri resourceUri);
    #endregion
}
