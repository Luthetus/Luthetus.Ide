using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using System;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

/// <summary>
/// <see cref="ITextEditorService.TextEditorViewModelApi"/>
/// </summary>
public class TextEditorViewModelApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.TextEditorViewModelApi(ITextEditorService, Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService, Fluxor.IState{RazorLib.TextEditors.States.TextEditorViewModelState}, Fluxor.IState{RazorLib.TextEditors.States.TextEditorModelState}, Microsoft.JSInterop.IJSRuntime, Fluxor.IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out var textEditorService,
            out var serviceProvider);

        Assert.NotNull(textEditorService.ViewModelApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorShouldBlink"/>
    /// </summary>
    [Fact]
    public void CursorShouldBlink()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorShouldBlinkChanged"/>
    /// </summary>
    [Fact]
    public void CursorShouldBlinkChanged()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.SetCursorShouldBlink(bool)"/>
    /// </summary>
    [Fact]
    public void SetCursorShouldBlink()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.WithValueEnqueue(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, Func{RazorLib.TextEditors.Models.TextEditorViewModel, RazorLib.TextEditors.Models.TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void WithValueEnqueue()
    {
        InitializeTextEditorViewModelApiTests(
            out var textEditorService,
            out var model,
            out var viewModel,
            out var serviceProvider);

        var oppositeShouldSetFocusAfterNextRender = !viewModel.ShouldSetFocusAfterNextRender;

        Assert.NotEqual(
            oppositeShouldSetFocusAfterNextRender,
            viewModel.ShouldSetFocusAfterNextRender);

        textEditorService.ViewModelApi.WithValueEnqueue(
            viewModel.ViewModelKey,
            inState => inState with
            {
                ShouldSetFocusAfterNextRender = oppositeShouldSetFocusAfterNextRender
            });

        var modifiedViewModel = textEditorService.ViewModelApi.GetOrDefault(viewModel.ViewModelKey);

        Assert.Equal(
            oppositeShouldSetFocusAfterNextRender,
            modifiedViewModel!.ShouldSetFocusAfterNextRender);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.SetScrollPositionEnqueue(string, string, double?, double?)"/>
    /// </summary>
    [Fact]
    public void SetScrollPositionEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.SetGutterScrollTopEnqueue(string, double)"/>
    /// </summary>
    [Fact]
    public void SetGutterScrollTopEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.Register(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, RazorLib.Lexes.Models.ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Register()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.MutateScrollVerticalPositionEnqueue(string, string, double)"/>
    /// </summary>
    [Fact]
    public void MutateScrollVerticalPositionEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.MutateScrollHorizontalPositionEnqueue(string, string, double)"/>
    /// </summary>
    [Fact]
    public void MutateScrollHorizontalPositionEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetModelOrDefault(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void FindBackingModelOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetAllText(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.FocusPrimaryCursorAsync(string)"/>
    /// </summary>
    [Fact]
    public void FocusPrimaryCursorAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetOrDefault(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void FindOrDefault()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetTextEditorMeasurementsAsync(string)"/>
    /// </summary>
    [Fact]
    public void GetTextEditorMeasurementsAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.MeasureCharacterWidthAndRowHeightAsync(string, int)"/>
    /// </summary>
    [Fact]
    public void MeasureCharacterWidthAndRowHeightAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.WithTaskEnqueue(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, Func{RazorLib.TextEditors.Models.TextEditorViewModel, Task{Func{RazorLib.TextEditors.Models.TextEditorViewModel, RazorLib.TextEditors.Models.TextEditorViewModel}}})"/>
    /// </summary>
    [Fact]
    public void WithTaskEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.MoveCursor(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs, RazorLib.Lexes.Models.ResourceUri, Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, Common.RazorLib.Keys.Models.Key{RazorLib.Cursors.Models.TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void MoveCursorEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorMovePageTopEnqueue(RazorLib.Lexes.Models.ResourceUri, Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, Common.RazorLib.Keys.Models.Key{RazorLib.Cursors.Models.TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageTopEnqueue()
    {

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorMovePageBottomEnqueue(RazorLib.Lexes.Models.ResourceUri, Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel}, Common.RazorLib.Keys.Models.Key{RazorLib.Cursors.Models.TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageBottomEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.Dispose(Common.RazorLib.Keys.Models.Key{RazorLib.TextEditors.Models.TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    private void InitializeTextEditorViewModelApiTests(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out IServiceProvider serviceProvider)
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServiceTests(
            out textEditorService,
            out serviceProvider);

        var fileExtension = ExtensionNoPeriodFacts.TXT;
        var resourceUri = new ResourceUri("/unitTesting.txt");
        var resourceLastWriteTime = DateTime.UtcNow;
        var initialContent = "Hello World!";

        textEditorService.ModelApi.RegisterTemplated(
            fileExtension,
            resourceUri,
            resourceLastWriteTime,
            initialContent);

        model = textEditorService.ModelApi.GetOrDefault(resourceUri)
           ?? throw new ArgumentNullException();

        var viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            resourceUri);

        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
           ?? throw new ArgumentNullException();
    }
}