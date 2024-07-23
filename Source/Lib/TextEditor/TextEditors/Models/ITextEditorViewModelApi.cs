using System.Collections.Immutable;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorViewModelApi
{
    #region CREATE_METHODS
    public void Register(
        Key<TextEditorViewModel> textEditorViewModelKey,
        ResourceUri resourceUri,
        Category category);
    #endregion

    #region READ_METHODS
    /// <summary>
    /// One should store the result of invoking this method in a variable, then reference that variable.
    /// If one continually invokes this, there is no guarantee that the data had not changed
    /// since the previous invocation.
    /// </summary>
    public ImmutableList<TextEditorViewModel> GetViewModels();
    public TextEditorModel? GetModelOrDefault(Key<TextEditorViewModel> viewModelKey);
    public Task<TextEditorDimensions> GetTextEditorMeasurementsAsync(string elementId);

    public Task<CharAndLineMeasurements> MeasureCharacterWidthAndLineHeightAsync(
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters);

    public TextEditorViewModel? GetOrDefault(Key<TextEditorViewModel> viewModelKey);
    public string? GetAllText(Key<TextEditorViewModel> viewModelKey);

    public void SetCursorShouldBlink(bool cursorShouldBlink);
    #endregion

    #region UPDATE_METHODS
    public TextEditorEditAsync WithValueFactory(
        Key<TextEditorViewModel> viewModelKey,
        Func<TextEditorViewModel, TextEditorViewModel> withFunc);

    public TextEditorEditAsync WithTaskFactory(
        Key<TextEditorViewModel> viewModelKey,
        Func<TextEditorViewModel, Task<Func<TextEditorViewModel, TextEditorViewModel>>> withFuncWrap);

    /// <summary>
    /// If a parameter is null the JavaScript will not modify that value
    /// </summary>
    public TextEditorEditAsync SetScrollPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double? scrollLeftInPixels,
        double? scrollTopInPixels);

    public TextEditorEditAsync ScrollIntoViewFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorTextSpan textSpan);

    public TextEditorEditAsync MutateScrollVerticalPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double pixels);

    public TextEditorEditAsync MutateScrollHorizontalPositionFactory(
        Key<TextEditorViewModel> viewModelKey,
        double pixels);

    public TextEditorEditAsync FocusPrimaryCursorFactory(string primaryCursorContentId);

    public TextEditorEditAsync MoveCursorFactory(
        KeyboardEventArgs keyboardEventArgs,
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="MoveCursorFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public TextEditorEditAsync MoveCursorUnsafeFactory(
        KeyboardEventArgs keyboardEventArgs,
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier primaryCursor);

    public TextEditorEditAsync CursorMovePageTopFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="CursorMovePageTopFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public TextEditorEditAsync CursorMovePageTopUnsafeFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier primaryCursor);

    public TextEditorEditAsync CursorMovePageBottomFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey);

    /// <summary>
    /// If one wants to guarantee that the state is up to date use <see cref="CursorMovePageBottomFactory"/>
    /// instead of this method. This is because, the <see cref="ITextEditorService"/> will provide
    /// you the latest instance of the given <see cref="TextEditorCursor"/>. As opposed to whatever
    /// instance of the <see cref="TextEditorCursorModifier"/> you have at time of enqueueing.
    /// <br/><br/>
    /// This method is needed however, because if one wants to arbitrarily create a cursor that does not
    /// map to the view model's cursors, then one would use this method. Since an attempt to map
    /// the cursor key would come back as the cursor not existing.
    /// </summary>
    public TextEditorEditAsync CursorMovePageBottomUnsafeFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorCursorModifier cursorModifier);

    public TextEditorEditAsync CalculateVirtualizationResultFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        CancellationToken cancellationToken);

    public TextEditorEditAsync RemeasureFactory(
        ResourceUri modelResourceUri,
        Key<TextEditorViewModel> viewModelKey,
        string measureCharacterWidthAndLineHeightElementId,
        int countOfTestCharacters,
        CancellationToken cancellationToken);

    public TextEditorEditAsync ForceRenderFactory(
        Key<TextEditorViewModel> viewModelKey,
        CancellationToken cancellationToken);
    #endregion

    #region DELETE_METHODS
    public void Dispose(Key<TextEditorViewModel> viewModelKey);
    #endregion

    public bool CursorShouldBlink { get; }
    public event Action? CursorShouldBlinkChanged;
}
