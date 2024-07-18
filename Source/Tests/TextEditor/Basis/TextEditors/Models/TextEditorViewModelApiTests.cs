using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.Tests.JsRuntimes;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models;

/// <summary>
/// <see cref="TextEditorViewModelApi"/>
/// </summary>
public class TextEditorViewModelApiTests : TextEditorTestBase
{
    /// <summary>
    /// <see cref="TextEditorViewModelApi(ITextEditorService, IBackgroundTaskService, IState{TextEditorViewModelState}, IState{TextEditorModelState}, IJSRuntime, IDispatcher, IDialogService)"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        Assert.NotNull(textEditorService.ViewModelApi);
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.CalculateVirtualizationResultFactory(ResourceUri, Key{TextEditorViewModel}, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void CalculateVirtualizationResultFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.CursorMovePageTopFactory(ResourceUri, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageTopFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.CursorMovePageTopUnsafeFactory(ResourceUri, Key{TextEditorViewModel}, TextEditorCursorModifier)"/>
    /// </summary>
    [Fact]
    public void CursorMovePageTopUnsafeFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.CursorMovePageBottomFactory(ResourceUri, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void CursorMovePageBottomFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.CursorMovePageBottomUnsafeFactory(ResourceUri, Key{TextEditorViewModel}, TextEditorCursorModifier)"/>
    /// </summary>
    [Fact]
    public void CursorMovePageBottomUnsafeFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.Dispose(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void Dispose()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ViewModelApi.GetViewModels());

        textEditorService.ViewModelApi.Dispose(inViewModel.ViewModelKey);

        Assert.Empty(textEditorService.ViewModelApi.GetViewModels());
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.FocusPrimaryCursorFactory(string)"/>
    /// </summary>
    [Fact]
    public void FocusPrimaryCursorFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.GetTextEditorMeasurementsAsync(string)"/>
    /// </summary>
    [Fact]
    public void GetTextEditorMeasurementsAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.GetModelOrDefault(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void GetModelOrDefault()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
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
    /// <see cref="TextEditorViewModelApi.GetAllText(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void GetAllText()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Equal(
            inModel.GetAllText(),
            textEditorService.ViewModelApi.GetAllText(inViewModel.ViewModelKey));
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.GetOrDefault(Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void GetOrDefault()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Equal(
            inViewModel,
            textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey));
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.GetViewModels()"/>
    /// </summary>
    [Fact]
    public void GetViewModels()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MutateScrollVerticalPositionFactory(string, string, double)"/>
    /// </summary>
    [Fact]
    public void MutateScrollVerticalPositionFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MutateScrollHorizontalPositionFactory(string, string, double)"/>
    /// </summary>
    [Fact]
    public void MutateScrollHorizontalPositionFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MeasureCharacterWidthAndLineHeightAsync(string, int)"/>
    /// </summary>
    [Fact]
    public void MeasureCharacterWidthAndLineHeightAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void MoveCursorFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 1 character
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            ShiftKey = true,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(1, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionAnchorPosition refers to the anchor position being smaller than the ending position.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionAnchorPosition_Then_MoveLeft()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
			async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            ShiftKey = true,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                }

                // Assert cursor's state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionEndingPosition refers to the ending position being smaller than the anchor.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionEndingPosition_Then_MoveLeft()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    // First, move the cursor 3 positions right.
                    // This test case calls for a small selection ending position.
                    // If one has the cursor start at (lineIndex 0, columnIndex 0), this isn't possible.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    }

                    // Assert cursor's state
                    Assert.Equal(1, primaryCursorModifier.LineIndex);
                    Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                    Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                    Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                    // Now one can select left, to get a small selection ending position.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                    }
                }

                // Assert cursor's state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionAnchorPosition refers to the anchor position being smaller than the ending position.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionAnchorPosition_Then_MoveDown()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            ShiftKey = true,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                }

                // Assert cursor's state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(2, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionEndingPosition refers to the ending position being smaller than the anchor.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionEndingPosition_Then_MoveDown()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    // First, move the cursor 3 positions right.
                    // This test case calls for a small selection ending position.
                    // If one has the cursor start at (lineIndex 0, columnIndex 0), this isn't possible.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    }

                    // Assert cursor's state
                    Assert.Equal(1, primaryCursorModifier.LineIndex);
                    Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                    Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                    Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                    // Now one can select left, to get a small selection ending position.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                    }
                }

                // Assert cursor's state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_DOWN,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionAnchorPosition refers to the anchor position being smaller than the ending position.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionAnchorPosition_Then_MoveUp()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            ShiftKey = true,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                }

                // Assert cursor's state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_UP,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                //
                // NOTE: LineIndex of 0 has length of 0 (not counting its line end),
                //       that is why the ColumnIndex lowered to 0.
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionEndingPosition refers to the ending position being smaller than the anchor.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionEndingPosition_Then_MoveUp()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    // First, move the cursor 3 positions right.
                    // This test case calls for a small selection ending position.
                    // If one has the cursor start at (lineIndex 0, columnIndex 0), this isn't possible.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    }

                    // Assert cursor's state
                    Assert.Equal(1, primaryCursorModifier.LineIndex);
                    Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                    Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                    Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                    // Now one can select left, to get a small selection ending position.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                    }
                }

                // Assert cursor's state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move left, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_UP,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionAnchorPosition refers to the anchor position being smaller than the ending position.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionAnchorPosition_Then_MoveRight()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            ShiftKey = true,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                }

                // Assert cursor's state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move right, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel})"/>
    /// ----------
    /// SmallSelectionEndingPosition refers to the ending position being smaller than the anchor.
    /// </summary>
    [Fact]
    public void MoveCursorFactory_SelectText_SmallSelectionEndingPosition_Then_MoveRight()
    {
        InitializeTextEditorViewModelApiTests(
            out var resourceUri, out var viewModelKey, out var textEditorService, out var serviceProvider);

        textEditorService.PostDistinct(
            nameof(MoveCursorFactory_SelectText),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // Assert cursor's starting state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Select 3 characters
                {
                    // First, move the cursor 3 positions right.
                    // This test case calls for a small selection ending position.
                    // If one has the cursor start at (lineIndex 0, columnIndex 0), this isn't possible.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                    }

                    // Assert cursor's state
                    Assert.Equal(1, primaryCursorModifier.LineIndex);
                    Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                    Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                    Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                    // Now one can select left, to get a small selection ending position.
                    {
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                            new KeyboardEventArgs
                            {
                                Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                ShiftKey = true,
                            },
                            resourceUri,
                            viewModelKey)
                        .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                        await textEditorService.ViewModelApi.MoveCursorFactory(
                                new KeyboardEventArgs
                                {
                                    Key = KeyboardKeyFacts.MovementKeys.ARROW_LEFT,
                                    ShiftKey = true,
                                },
                                resourceUri,
                                viewModelKey)
                            .Invoke(editContext);
                    }
                }

                // Assert cursor's state
                Assert.Equal(0, primaryCursorModifier.LineIndex);
                Assert.Equal(0, primaryCursorModifier.ColumnIndex);
                Assert.Equal(3, primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);

                // Move right, no shift key.
                await textEditorService.ViewModelApi.MoveCursorFactory(
                        new KeyboardEventArgs
                        {
                            Key = KeyboardKeyFacts.MovementKeys.ARROW_RIGHT,
                        },
                        resourceUri,
                        viewModelKey)
                    .Invoke(editContext);

                // Assert cursor's ending state
                Assert.Equal(1, primaryCursorModifier.LineIndex);
                Assert.Equal(2, primaryCursorModifier.ColumnIndex);
                Assert.Null(primaryCursorModifier.SelectionAnchorPositionIndex);
                Assert.Equal(0, primaryCursorModifier.SelectionEndingPositionIndex);
            });
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.MoveCursorUnsafeFactory(KeyboardEventArgs, ResourceUri, Key{TextEditorViewModel}, TextEditorCursorModifier)"/>
    /// </summary>
    [Fact]
    public void MoveCursorUnsafeFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.Register(Key{TextEditorViewModel}, ResourceUri, Category)"/>
    /// </summary>
    [Fact]
    public void Register()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        Assert.Single(textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri));

        textEditorService.ViewModelApi.Register(
            Key<TextEditorViewModel>.NewKey(),
            inModel.ResourceUri,
            new Category("UnitTesting"));

        Assert.Equal(2, textEditorService.ModelApi.GetViewModelsOrEmpty(inModel.ResourceUri).Length);
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.RemeasureFactory(ResourceUri, Key{TextEditorViewModel}, string, int, CancellationToken)"/>
    /// </summary>
    [Fact]
    public void RemeasureFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.ScrollIntoViewFactory(string, string, int?, int?)"/>
    /// </summary>
    [Fact]
    public void ScrollIntoViewFactory_LineIndex_ColumnIndex()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.ScrollIntoViewFactory(string, string, int)"/>
    /// </summary>
    [Fact]
    public void ScrollIntoViewFactory_PositionIndex()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// <see cref="TextEditorViewModelApi.ScrollIntoViewFactory(ResourceUri, Key{TextEditorViewModel}, TextEditorTextSpan)"/>
    /// </summary>
    [Fact]
    public void ScrollIntoViewFactory_TextSpan()
    {
        throw new NotImplementedException();
    }
    
    /// <summary>
    /// <see cref="TextEditorViewModelApi.SetScrollPositionFactory(string, string, double?, double?)"/>
    /// </summary>
    [Fact]
    public void SetScrollPositionFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.SetGutterScrollTopFactory(string, double)"/>
    /// </summary>
    [Fact]
    public void SetGutterScrollTopFactory()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.SetCursorShouldBlink(bool)"/>
    /// <br/>----<br/>
    /// <see cref="TextEditorViewModelApi.CursorShouldBlink"/>
    /// <see cref="TextEditorViewModelApi.CursorShouldBlinkChanged"/>
    /// </summary>
    [Fact]
    public async Task SetCursorShouldBlink()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
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
    /// <see cref="TextEditorViewModelApi.WithValueFactory(Key{TextEditorViewModel}, Func{TextEditorViewModel, TextEditorViewModel})"/>
    /// </summary>
    [Fact]
    public void WithValueFactory()
    {
        TestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var oppositeShouldSetFocusAfterNextRender = !inViewModel.UnsafeState.ShouldSetFocusAfterNextRender;

        Assert.NotEqual(
            oppositeShouldSetFocusAfterNextRender,
            inViewModel.UnsafeState.ShouldSetFocusAfterNextRender);

        textEditorService.PostDistinct(
            nameof(textEditorService.ViewModelApi.WithValueFactory),
            textEditorService.ViewModelApi.WithValueFactory(
                inViewModel.ViewModelKey,
                inState =>
                {
                    inState.UnsafeState.ShouldSetFocusAfterNextRender = oppositeShouldSetFocusAfterNextRender;
                    return inState with { };
                }));

        var modifiedViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);

        Assert.Equal(
            oppositeShouldSetFocusAfterNextRender,
            modifiedViewModel!.UnsafeState.ShouldSetFocusAfterNextRender);
    }

    /// <summary>
    /// <see cref="TextEditorViewModelApi.WithTaskFactory(Key{TextEditorViewModel}, Func{TextEditorViewModel, Task{Func{TextEditorViewModel, TextEditorViewModel}}})"/>
    /// </summary>
    [Fact]
    public void WithTaskFactory()
    {
        throw new NotImplementedException();
    }

    private void InitializeTextEditorViewModelApiTests(
        out ResourceUri resourceUri,
        out Key<TextEditorViewModel> viewModelKey,
        out ITextEditorService textEditorService,
        out IServiceProvider serviceProvider)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var services = new ServiceCollection()
            .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
            .AddLuthetusTextEditor(new LuthetusHostingInformation(LuthetusHostingKind.UnitTestingSynchronous, backgroundTaskService))
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly));

        serviceProvider = services.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        var (inModel, modelModifier) = NotEmptyEditor_TestData_And_PerformPreAssertions(
            resourceUri: new ResourceUri($"/{nameof(NotEmptyEditor_TestData_And_PerformPreAssertions)}.txt"),
            resourceLastWriteTime: DateTime.MinValue,
            fileExtension: ExtensionNoPeriodFacts.TXT,
            content: "\nb9\r9B\r\n\t$; ",
            decorationMapper: null,
            compilerService: null,
            partitionSize: 4096);
        
        textEditorService.ModelApi.RegisterCustom(inModel);

        resourceUri = inModel.ResourceUri;

        viewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            viewModelKey,
            resourceUri,
            new Category("UnitTesting"));

        var viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey) ?? throw new ArgumentNullException();
    }
}