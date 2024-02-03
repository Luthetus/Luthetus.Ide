using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Xunit;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

/// <summary>
/// <see cref="ITextEditorService.TextEditorViewModelApi"/>
/// </summary>
public class TextEditorViewModelApiTests
{
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.TextEditorViewModelApi(ITextEditorService, Common.RazorLib.BackgroundTasks.Models.IBackgroundTaskService, Fluxor.IState{TextEditorViewModelState}, Fluxor.IState{TextEditorModelState}, Microsoft.JSInterop.IJSRuntime, Fluxor.IDispatcher)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.NotNull(textEditorService.ViewModelApi);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorShouldBlink"/>
    /// <br/>----<br/>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorShouldBlinkChanged"/>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.SetCursorShouldBlink(bool)"/>
    /// </summary>
    [Fact]
    public async Task CursorShouldBlink()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var cursorShouldBlinkChangedCount = 0;
        var expectedChangeCount = 2;
        var expectedBoolean = false;

        // Delay for blinking cursor is 1,000 (miliseconds)
        // The blinking cursor is done via a fire and forget Task.Run.
        // Therefore I'm going to test using a polling solution.
        // TODO: This test is hacky. (2023-12-17)
        var checkResultDelay = TimeSpan.FromMilliseconds(1_100);

        void ViewModelApi_CursorShouldBlinkChanged()
        {
            cursorShouldBlinkChangedCount++;
            Assert.Equal(expectedBoolean, textEditorService.ViewModelApi.CursorShouldBlink);
        }

        var checkResultTask = Task.Run(async () =>
        {
            while (cursorShouldBlinkChangedCount != expectedChangeCount)
            {
                await Task.Delay(checkResultDelay);
            }
        });

        textEditorService.ViewModelApi.CursorShouldBlinkChanged += ViewModelApi_CursorShouldBlinkChanged;

        expectedBoolean = false;
        textEditorService.ViewModelApi.SetCursorShouldBlink(expectedBoolean);

        expectedBoolean = true;
        textEditorService.ViewModelApi.SetCursorShouldBlink(expectedBoolean);

        await checkResultTask;
        textEditorService.ViewModelApi.CursorShouldBlinkChanged -= ViewModelApi_CursorShouldBlinkChanged;
        Assert.Equal(expectedChangeCount, cursorShouldBlinkChangedCount);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.WithValueEnqueue(Key{TextEditorViewModel}, Func{TextEditorViewModel, TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void WithValueEnqueue()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var oppositeShouldSetFocusAfterNextRender = !inViewModel.ShouldSetFocusAfterNextRender;

        Assert.NotEqual(
            oppositeShouldSetFocusAfterNextRender,
            inViewModel.ShouldSetFocusAfterNextRender);

        textEditorService.Post(
            nameof(textEditorService.ViewModelApi.WithValueFactory),
            textEditorService.ViewModelApi.WithValueFactory(
                inViewModel.ViewModelKey,
                inState => inState with
                {
                    ShouldSetFocusAfterNextRender = oppositeShouldSetFocusAfterNextRender
                }));

        var modifiedViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);

        Assert.Equal(
            oppositeShouldSetFocusAfterNextRender,
            modifiedViewModel!.ShouldSetFocusAfterNextRender);
    }
    
    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.Register(Key{TextEditorViewModel}, ResourceUri)"/>
    /// </summary>
    [Fact]
    public void Register()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));

        textEditorService.ViewModelApi.Register(
            Key<TextEditorViewModel>.NewKey(),
            inModel.ResourceUri,
            new TextEditorCategory("UnitTesting"));

        Assert.Equal(2, textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri).Length);
    }/// <summary>
     /// <see cref="ITextEditorService.TextEditorViewModelApi.GetModelOrDefault(Key{TextEditorViewModel})"/>
     /// </summary>
    [Fact]
    public void GetModelOrDefault()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var viewModelsOrEmpty = textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri);
        var getModelOrDefault = textEditorService.ViewModelApi.GetModelOrDefault(inViewModel.ViewModelKey);

        Assert.Single(viewModelsOrEmpty);
        Assert.Equal(inViewModel, viewModelsOrEmpty.Single());

        Assert.Equal(inModel, getModelOrDefault);
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetAllText(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Equal(
            inModel.GetAllText(),
            textEditorService.ViewModelApi.GetAllText(inViewModel.ViewModelKey));
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.GetOrDefault(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void FindOrDefault()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Equal(
            inViewModel,
            textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey));
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.Dispose(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ViewModelApi.GetViewModels());

        textEditorService.ViewModelApi.Dispose(inViewModel.ViewModelKey);

        Assert.Empty(textEditorService.ViewModelApi.GetViewModels());
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.WithTaskEnqueue(Key{TextEditorViewModel}, Func{TextEditorViewModel, Task{Func{TextEditorViewModel, TextEditorViewModel}}})"/>
    /// </summary>
    [Fact]
    public void WithTaskEnqueue()
    {
        throw new NotImplementedException();
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
    /// <see cref="ITextEditorService.TextEditorViewModelApi.FocusPrimaryCursorFactory(string)"/>
    /// </summary>
    [Fact]
    public void FocusPrimaryCursorAsync()
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
    /// <see cref="ITextEditorService.TextEditorViewModelApi.MoveCursorEnqueue(Microsoft.AspNetCore.Components.Web.KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel}, Key{TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void MoveCursorEnqueue()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorMovePageTopEnqueue(ResourceUri, Key{TextEditorViewModel}, Key{TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageTopEnqueue()
    {

        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="ITextEditorService.TextEditorViewModelApi.CursorMovePageBottomEnqueue(ResourceUri, Key{TextEditorViewModel}, Key{TextEditorCursor})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageBottomEnqueue()
    {
        throw new NotImplementedException();
    }
}