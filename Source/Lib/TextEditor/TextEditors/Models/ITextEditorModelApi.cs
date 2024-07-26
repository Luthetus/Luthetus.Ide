using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorModelApi
{
    #region CREATE_METHODS
    /// <summary>It is recommended to use the <see cref="RegisterTemplated" /> method as it will internally reference the <see cref="ITextEditorLexer" /> and <see cref="IDecorationMapper" /> that correspond to the desired text editor.</summary>
    public void RegisterCustom(TextEditorModel model);
    /// <summary>
    /// Plain Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
    /// of "txt" or the constant varible: <see cref="ExtensionNoPeriodFacts.TXT"/>.<br /><br />
    /// 
    /// C# Text Editor: one would pass in an <see cref="extensionNoPeriod"/>
    /// of "cs" or the constant varible: <see cref="ExtensionNoPeriodFacts.C_SHARP_CLASS"/>;
    /// NOTE: One must first install the Luthetus.CompilerServices.CSharp NuGet package.<br /><br />
    /// </summary>
    public void RegisterTemplated(
        string extensionNoPeriod,
        ResourceUri resourceUri,
        DateTime resourceLastWriteTime,
        string initialContent,
        string? overrideDisplayTextForFileExtension = null);
    #endregion

    #region READ_METHODS
    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public ImmutableList<TextEditorModel> GetModels();
    public TextEditorModel? GetOrDefault(ResourceUri resourceUri);
    public ImmutableArray<TextEditorViewModel> GetViewModelsOrEmpty(ResourceUri resourceUri);
    public string? GetAllText(ResourceUri resourceUri);
    #endregion

    #region UPDATE_METHODS
    public void UndoEdit(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier);

    public void SetUsingLineEndKind(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        LineEndKind lineEndKind);

    public void SetResourceData(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        DateTime resourceLastWriteTime);

    public void Reload(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        string content,
        DateTime resourceLastWriteTime);

    public void RedoEdit(
    	ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier);

    public void InsertText(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken);

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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        string content,
        CancellationToken cancellationToken);

    public void HandleKeyboardEvent(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken);

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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        KeyboardEventArgs keyboardEventArgs,
        CancellationToken cancellationToken);

    public void DeleteTextByRange(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken);

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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        int count,
        CancellationToken cancellationToken);

    public void DeleteTextByMotion(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken);

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
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        CursorModifierBagTextEditor cursorModifierBag,
        MotionKind motionKind,
        CancellationToken cancellationToken);

    public void AddPresentationModel(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        TextEditorPresentationModel emptyPresentationModel);

    /// <param name="emptyPresentationModel">
    /// If the presentation model was not found, the empty presentation model will be registered.
    /// </param>
    public void StartPendingCalculatePresentationModel(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel);

    /// <param name="emptyPresentationModel">
    /// If the presentation model was not found, the empty presentation model will be registered.
    /// </param>
    public void CompletePendingCalculatePresentationModel(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        Key<TextEditorPresentationModel> presentationKey,
        TextEditorPresentationModel emptyPresentationModel,
        ImmutableArray<TextEditorTextSpan> calculatedTextSpans);

    /// <summary>
    /// If applying syntax highlighting it may be preferred to use <see cref="ApplySyntaxHighlightingAsync" />.
    /// It is effectively just invoking the lexer and then <see cref="ApplyDecorationRange" />
    /// </summary>
    public void ApplyDecorationRange(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier,
        IEnumerable<TextEditorTextSpan> textSpans);

    public void ApplySyntaxHighlighting(
        ITextEditorEditContext editContext,
        TextEditorModelModifier modelModifier);
    #endregion

    #region DELETE_METHODS
    public void Dispose(ResourceUri resourceUri);
    #endregion
}
