using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models;
using Luthetus.TextEditor.RazorLib;

namespace Luthetus.TextEditor.Tests.Basis.Commands.Models.Defaults;

/// <summary>
/// <see cref="TextEditorCommandDefaultFacts"/>
/// </summary>
public class TextEditorCommandDefaultFactsTests
{
	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.DoNothingDiscard"/>
	/// </summary>
	[Fact]
    public async Task DoNothingDiscard()
    {
        InitializeTextEditorCommandDefaultFactsTests(
            out var textEditorService, out var inModel, out var inViewModel,
            out var textEditorCommandArgs, out var serviceProvider);

		await TextEditorCommandDefaultFacts.DoNothingDiscard.CommandFunc.Invoke(
            textEditorCommandArgs);
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Copy"/>
	/// </summary>
	[Fact]
    public async Task Copy()
    {
        InitializeTextEditorCommandDefaultFactsTests(
            out var textEditorService, out var inModel, out var inViewModel,
            out var textEditorCommandArgs, out var serviceProvider);

        var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

        // No selection
        {
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Hello World!\n", outClipboard);
        }


        // With selection
        {
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            textEditorService.PostIndependent(
				nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
				{
					var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
					var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
					var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
					var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

					if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
						return Task.CompletedTask;

					primaryCursorModifier.LineIndex = 1;
					primaryCursorModifier.SetColumnIndexAndPreferred(9);
					primaryCursorModifier.SelectionAnchorPositionIndex = 15;
					primaryCursorModifier.SelectionEndingPositionIndex = 22;

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.Copy.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Pillows", outClipboard);
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Cut"/>
	/// </summary>
	[Fact]
	public async Task Cut()
	{
        // No selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(textEditorCommandArgs);

            var outClipboard = await clipboardService.ReadClipboard();
            Assert.Equal("Hello World!\n", outClipboard);

            // Assert text was deleted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("7 Pillows\n \n,abc123", outText);
            }
        }

        // With selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();
            await clipboardService.SetClipboard(string.Empty);
            var inClipboard = await clipboardService.ReadClipboard();
            Assert.Empty(inClipboard);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(9);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 15;
                    primaryCursorModifier.SelectionEndingPositionIndex = 22;

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.Cut.CommandFunc.Invoke(textEditorCommandArgs);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var outClipboard = await clipboardService.ReadClipboard();
                    Assert.Equal("Pillows", outClipboard);

                    // Assert text was deleted
                    {
                        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                        Assert.NotNull(outModel);

                        var outText = outModel!.GetAllText();
                        Assert.Equal("Hello World!\n7 \n \n,abc123", outText);
                    }
                });
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.PasteCommand"/>
	/// </summary>
	[Fact]
    public async Task Paste()
    {
        // No selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return;

                    var stringToPaste = "Alphabet Soup\n";
                    await clipboardService.SetClipboard(stringToPaste);
                    Assert.Equal(stringToPaste, await clipboardService.ReadClipboard());

                    primaryCursorModifier.LineIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);

                    return;
                });

            await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert text was pasted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("Alphabet Soup\nHello World!\n7 Pillows\n \n,abc123", outText);
            }
        }

        // With selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var clipboardService = serviceProvider.GetRequiredService<IClipboardService>();

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return;

                    var stringToPaste = "Alphabet Soup\n";
                    await clipboardService.SetClipboard(stringToPaste);
                    Assert.Equal(stringToPaste, await clipboardService.ReadClipboard());

                    primaryCursorModifier.LineIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);

                    // Select the first row in its entirety (including line ending)
                    {
                        primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                        primaryCursorModifier.SelectionEndingPositionIndex = 13;
                    }

                    return;
                });

            await TextEditorCommandDefaultFacts.PasteCommand.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert text was pasted
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var outText = outModel!.GetAllText();
                Assert.Equal("Alphabet Soup\n7 Pillows\n \n,abc123", outText);
            }
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Save"/>
	/// </summary>
	[Fact]
    public async Task Save()
    {
        // ViewModel.OnSaveRequested is NOT null
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var savedContent = (string?)null;
            Assert.Null(savedContent);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    viewModelModifier.ViewModel = viewModelModifier.ViewModel with
                    {
                        OnSaveRequested = model => savedContent = model.GetAllText()
                    };

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.Save.CommandFunc.Invoke(textEditorCommandArgs);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    Assert.Equal(TestConstants.SOURCE_TEXT, savedContent);
                    return Task.CompletedTask;
                });
        }

        // ViewModel.OnSaveRequested is null
        {
                InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

                var savedContent = (string?)null;

                Assert.Null(savedContent);
                await TextEditorCommandDefaultFacts.Save.CommandFunc.Invoke(textEditorCommandArgs);

                textEditorService.PostIndependent(
                    nameof(TextEditorCommandDefaultFactsTests),
                    editContext =>
                    {
                        Assert.Null(savedContent);
                        return Task.CompletedTask;
                    });
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.SelectAll"/>
	/// </summary>
	[Fact]
    public async Task SelectAll()
    {
        // No already existing selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            await TextEditorCommandDefaultFacts.SelectAll.CommandFunc.Invoke(textEditorCommandArgs);

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);

            Assert.Equal(0, outViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.Equal(inModel.DocumentLength, outViewModel!.PrimaryCursor.Selection.EndingPositionIndex);

            // Assert that 'SelectAll' does not move the cursor itself, it only should move the selection
            {
                Assert.Equal(inViewModel.PrimaryCursor.LineIndex, outViewModel!.PrimaryCursor.LineIndex);
                Assert.Equal(inViewModel.PrimaryCursor.ColumnIndex, outViewModel!.PrimaryCursor.ColumnIndex);
            }
        }

        // With an already existing selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    // Select the "Hello" text on the first row
                    {
                        primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                        primaryCursorModifier.SelectionEndingPositionIndex = 5;
                    }

                    // SetColumnIndexAndPreferred is unnecessary, but the user is likely
                    // to have their ColumnIndex == SelectionEndingPositionIndex
                    primaryCursorModifier.SetColumnIndexAndPreferred(
                        primaryCursorModifier.SelectionEndingPositionIndex);

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.SelectAll.CommandFunc.Invoke(textEditorCommandArgs);

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);

            Assert.Equal(0, outViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.Equal(inModel.DocumentLength, outViewModel!.PrimaryCursor.Selection.EndingPositionIndex);
        }
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Undo"/>
	/// </summary>
	[Fact]
    public async Task Undo()
    {
        // Able to undo
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            // Modify text, as to create an edit which one can then undo
            {
                var cursor = new TextEditorCursor(0, 0, true);

                var cursorModificationBag = new CursorModifierBagTextEditor(
                    Key<TextEditorViewModel>.Empty,
                    new List<TextEditorCursorModifier> { new TextEditorCursorModifier(cursor) });

                textEditorService.PostIndependent(
                    nameof(TextEditorCommandDefaultFactsTests),
                    textEditorService.ModelApi.InsertTextUnsafeFactory(
                        inModel.ResourceUri,
                        cursorModificationBag,
                        "zyx",
                        CancellationToken.None));
            }


            // Assert that the new text is different from the original
            {
                var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(refModel);

                var inText = inModel.GetAllText();
                var refText = refModel!.GetAllText();
                Assert.NotEqual(inText, refText);
            }
            
            // Invoke the command
            await TextEditorCommandDefaultFacts.Undo.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the text was reverted to the original
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var inText = inModel.GetAllText();
                var outText = outModel!.GetAllText();
                Assert.Equal(inText, outText);
            }
        }

        // Cannot undo
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            // Capture the current text, as to later be used after the
            // Undo invocation to Assert the text had not changed.
            var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(refModel);
            var refText = refModel!.GetAllText();

            // Invoke the command
            await TextEditorCommandDefaultFacts.Undo.CommandFunc.Invoke(textEditorCommandArgs);

            // Capture the text, now that the Undo command was invoked.
            var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(outModel);
            var outText = outModel!.GetAllText();

            // Assert that the text immediately BEFORE invoking the 'Undo' command
            // is equal to the text which one gets AFTER invoking 'Undo'
            //
            // This asserts that the 'cannot undo' case does not modify the text in any way.
            Assert.Equal(refText, outText);
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Redo"/>
	/// </summary>
	[Fact]
    public Task Redo()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //// Able to redo
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    // Modify text, as to create an edit which one can then undo
        //    {
        //        var cursor = new TextEditorCursor(0, 0, true);

        //        var cursorModificationBag = new TextEditorCursorModifierBag(
        //            Key<TextEditorViewModel>.Empty,
        //            new List<TextEditorCursorModifier> { new TextEditorCursorModifier(cursor) });

        //        textEditorService.Post(
        //            nameof(TextEditorCommandDefaultFactsTests),
        //            textEditorService.ModelApi.InsertTextUnsafeFactory(
        //                inModel.ResourceUri,
        //                cursorModificationBag,
        //                "zyx",
        //                CancellationToken.None));
        //    }

        //    // Capture the current text, as to later be used after the
        //    // Redo invocation to Assert the text has been re-edited.
        //    var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //    Assert.NotNull(refModel);
        //    var refText = refModel!.GetAllText();

        //    // Undo the previous edit, as to be able to redo an edit
        //    {
        //        await TextEditorCommandDefaultFacts.Undo.CommandFunc.Invoke(textEditorCommandArgs);
        //    }

        //    // Redo the previous edit
        //    {
        //        await TextEditorCommandDefaultFacts.Redo.CommandFunc.Invoke(textEditorCommandArgs);
        //    }

        //    // Assert that the text was re-edited
        //    {
        //        var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //        Assert.NotNull(outModel);

        //        var outText = outModel!.GetAllText();
        //        Assert.Equal(refText, outText);
        //    }
        //}

        //// Cannot redo
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    // Capture the current text, as to later be used after the
        //    // Redo invocation to Assert the text had not changed.
        //    var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //    Assert.NotNull(refModel);
        //    var refText = refModel!.GetAllText();

        //    // Invoke the command
        //    await TextEditorCommandDefaultFacts.Redo.CommandFunc.Invoke(textEditorCommandArgs);

        //    // Capture the text, now that the Redo command was invoked.
        //    var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //    Assert.NotNull(outModel);
        //    var outText = outModel!.GetAllText();

        //    // Assert that the text immediately BEFORE invoking the 'Redo' command
        //    // is equal to the text which one gets AFTER invoking 'Redo'
        //    //
        //    // This asserts that the 'cannot redo' case does not modify the text in any way.
        //    Assert.Equal(refText, outText);
        //}
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Duplicate"/>
	/// </summary>
	[Fact]
    public Task Duplicate()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //// No selection
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    // Duplicate with the default Cursor position. This should duplicate the
        //    // first row, including its line endings.
        //    await TextEditorCommandDefaultFacts.Duplicate.CommandFunc.Invoke(textEditorCommandArgs);

        //    var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //    Assert.NotNull(outModel);
        //    var outText = outModel!.GetAllText();
        //    Assert.Equal("Hello World!\n" + TestConstants.SOURCE_TEXT, outText);
        //}

        //// With selection
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    textEditorService.Post(
        //        nameof(TextEditorCommandDefaultFactsTests),
        //        editContext =>
        //        {
        //            var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
        //            var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
        //            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        //            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        //            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
        //                return Task.CompletedTask;

        //            primaryCursorModifier.RowIndex = 1;
        //            primaryCursorModifier.SetColumnIndexAndPreferred(9);
        //            primaryCursorModifier.SelectionAnchorPositionIndex = 15;
        //            primaryCursorModifier.SelectionEndingPositionIndex = 22;

        //            return Task.CompletedTask;
        //        });

        //    // Duplicate with the text selected.
        //    // This should duplicate the text 'Pillows' on the second row.
        //    await TextEditorCommandDefaultFacts.Duplicate.CommandFunc.Invoke(textEditorCommandArgs);

        //    var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
        //    Assert.NotNull(outModel);
        //    var outText = outModel!.GetAllText();
        //    Assert.Equal(TestConstants.SOURCE_TEXT.Insert(22, "Pillows"), outText);
        //}
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.IndentMore"/>
	/// </summary>
	[Fact]
    public async Task IndentMore()
    {
        // Invoke IndentMore on 1 row, by selecting a single character,
        // where the character is NOT at the start or end of the row
        //
        // Reasoning: In Luthetus.Ide, this should IndentMore the line on which the selection lies.
        //            This behavior is not universal however.
        //            In Visual Studio this will delete the user's selected text, and
        //            then insert a single tab character.
        //            
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(3);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 15;
                    primaryCursorModifier.SelectionEndingPositionIndex = 16;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("P", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentMore.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the row(s) were indented: "7 Pillows\n" -> "\t7 Pillows\n"
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var textOnRow = new string(outModel!
                    .GetLineRichCharacterRange(1, 1)
                    .Single()
                    .Select(x => x.Value)
                    .ToArray());

                Assert.Equal("\t7 Pillows\n", textOnRow);
            }

            // Assert that the viewModel's cursor was moved properly (including its selection)
            {
                var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
                Assert.NotNull(outViewModel);
                var primaryCursor = outViewModel!.PrimaryCursor;

                // The row index should be unchanged
                Assert.Equal(1, primaryCursor.LineIndex);

                Assert.Equal(4, primaryCursor.ColumnIndex);
                // Assert column and preferred indices to ensure both were set,
                // as opposed to only ColumnIndex and forgetting to also change PreferredColumnIndex.
                Assert.Equal(primaryCursor.ColumnIndex, primaryCursor.PreferredColumnIndex);

                // Due to the insertion of a '\t' character, the selection should move 1 character further
                Assert.Equal(16, primaryCursor.Selection.AnchorPositionIndex);
                Assert.Equal(17, primaryCursor.Selection.EndingPositionIndex);
            }
        }

        // Invoke IndentMore on 1 row, by selecting the entire row, including its line ending.
        //
        // Reasoning: This should IndentMore a single row.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 13;
                    primaryCursorModifier.SelectionEndingPositionIndex = 23;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("7 Pillows\n", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentMore.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the row(s) were indented: "7 Pillows\n" -> "\t7 Pillows\n"
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var textOnRow = new string(outModel!
                    .GetLineRichCharacterRange(1, 1)
                    .Single()
                    .Select(x => x.Value)
                    .ToArray());

                Assert.Equal("\t7 Pillows\n", textOnRow);
            }

            // Assert that the viewModel's cursor was moved properly (including its selection)
            {
                var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
                Assert.NotNull(outViewModel);
                var primaryCursor = outViewModel!.PrimaryCursor;

                // The row index should be unchanged
                Assert.Equal(1, primaryCursor.LineIndex);

                Assert.Equal(1, primaryCursor.ColumnIndex);
                // Assert column and preferred indices to ensure both were set,
                // as opposed to only ColumnIndex and forgetting to also change PreferredColumnIndex.
                Assert.Equal(primaryCursor.ColumnIndex, primaryCursor.PreferredColumnIndex);

                // Due to the insertion of a '\t' character, the selection should move 1 character further
                Assert.Equal(14, primaryCursor.Selection.AnchorPositionIndex);
                Assert.Equal(24, primaryCursor.Selection.EndingPositionIndex);
            }
        }

        // Invoke IndentMore on 2 rows, by selecting the entirety of one row (including its line ending),
        // and some but NOT all of another row.
        //
        // Reasoning: This should IndentMore a 2 rows.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    primaryCursorModifier.LineIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                    primaryCursorModifier.SelectionEndingPositionIndex = 16;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("Hello World!\n7 P", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentMore.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the row(s) were indented:
            //     "Hello World!\n7 Pillows\n" -> "\tHello World!\n\t7 Pillows\n"
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var textOnRow = string.Join(string.Empty, outModel!
                    .GetLineRichCharacterRange(0, 2)
                    .SelectMany(x => new string(x.Select(y => y.Value).ToArray()))
                    .ToArray());

                Assert.Equal("\tHello World!\n\t7 Pillows\n", textOnRow);
            }

            // Assert that the viewModel's cursor was moved properly (including its selection)
            {
                var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
                Assert.NotNull(outViewModel);
                var primaryCursor = outViewModel!.PrimaryCursor;

                // The row index should be unchanged
                Assert.Equal(0, primaryCursor.LineIndex);

                Assert.Equal(1, primaryCursor.ColumnIndex);
                // Assert column and preferred indices to ensure both were set,
                // as opposed to only ColumnIndex and forgetting to also change PreferredColumnIndex.
                Assert.Equal(primaryCursor.ColumnIndex, primaryCursor.PreferredColumnIndex);

                // Due to the insertion of a '\t' character, the selection should move 1 character further
                Assert.Equal(1, primaryCursor.Selection.AnchorPositionIndex);
                Assert.Equal(18, primaryCursor.Selection.EndingPositionIndex);
            }
        }

        // Invoke IndentMore on 2 rows, by selecting the entirety of one row (including its line ending),
        // and the entirety of another row (including its line ending).
        //
        // Reasoning: This should IndentMore a 2 rows.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    primaryCursorModifier.LineIndex = 0;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                    primaryCursorModifier.SelectionEndingPositionIndex = 23;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("Hello World!\n7 Pillows\n", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentMore.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the row(s) were indented:
            //     "Hello World!\n7 Pillows\n" -> "\tHello World!\n\t7 Pillows\n"
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);

                var textOnRow = string.Join(string.Empty, outModel!
                    .GetLineRichCharacterRange(0, 2)
                    .SelectMany(x => new string(x.Select(y => y.Value).ToArray()))
                    .ToArray());

                Assert.Equal("\tHello World!\n\t7 Pillows\n", textOnRow);
            }

            // Assert that the viewModel's cursor was moved properly (including its selection)
            {
                var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
                Assert.NotNull(outViewModel);
                var primaryCursor = outViewModel!.PrimaryCursor;

                // The row index should be unchanged
                Assert.Equal(0, primaryCursor.LineIndex);

                Assert.Equal(1, primaryCursor.ColumnIndex);
                // Assert column and preferred indices to ensure both were set,
                // as opposed to only ColumnIndex and forgetting to also change PreferredColumnIndex.
                Assert.Equal(primaryCursor.ColumnIndex, primaryCursor.PreferredColumnIndex);

                // Due to the insertion of a '\t' character, the selection should move 1 character further
                Assert.Equal(1, primaryCursor.Selection.AnchorPositionIndex);
                Assert.Equal(25, primaryCursor.Selection.EndingPositionIndex);
            }
        }

        // Invoke IndentMore without a selection.
        //
        // Reasoning: This should NOT IndentMore.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            await TextEditorCommandDefaultFacts.IndentMore.CommandFunc.Invoke(textEditorCommandArgs);

            // Assert that the row(s) were NOT indented
            {
                var inText = inModel.GetAllText();

                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);
                var outText = outModel!.GetAllText();

                Assert.Equal(inText, outText);
            }

            // Assert that the viewModel's cursor was moved properly (including its selection)
            {
                var inPrimaryCursor = inViewModel.PrimaryCursor;

                var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
                Assert.NotNull(outViewModel);
                var outPrimaryCursor = outViewModel!.PrimaryCursor;

                Assert.True(inPrimaryCursor == outPrimaryCursor);
            }
        }
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.IndentLess"/>
	/// </summary>
	[Fact]
    public async Task IndentLess()
    {
        // Invoke IndexLess on a single line of which the line has one tab character at the start.
        // Only select a single character.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                async editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return;

                    // Insert a tab character at the start of the row under testing
                    {
                        var cursor = new TextEditorCursor(1, 0, true);
                        var cursorModifier = new TextEditorCursorModifier(cursor);

                        var insertionCursorModifierBag = new CursorModifierBagTextEditor(
                            Key<TextEditorViewModel>.Empty,
                            new List<TextEditorCursorModifier> { cursorModifier });

                        await textEditorService.ModelApi.InsertTextUnsafeFactory(
                                modelModifier.ResourceUri,
                                insertionCursorModifierBag,
                                "\t",
                                CancellationToken.None)
                            .Invoke(editContext);

                        var rowText = new string(
                            modelModifier.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());

                        Assert.Equal("\t7 Pillows\n", rowText);
                    }

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(4);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 16;
                    primaryCursorModifier.SelectionEndingPositionIndex = 17;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("P", selectedText);
                    }

                    return;
                });

            await TextEditorCommandDefaultFacts.IndentLess.CommandFunc.Invoke(textEditorCommandArgs);

            var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(outModel);

            var rowText = new string(
                outModel!.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());
            
            Assert.Equal("7 Pillows\n", rowText);
        }

        // Invoke IndexLess on a single line, of which the line has 4 space characters at the start.
        // Only select a single character.
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    // Insert a tab character at the start of the row under testing
                    {
                        var cursor = new TextEditorCursor(1, 0, true);
                        var cursorModifier = new TextEditorCursorModifier(cursor);

                        var insertionCursorModifierBag = new CursorModifierBagTextEditor(
                            Key<TextEditorViewModel>.Empty,
                            new List<TextEditorCursorModifier> { cursorModifier });

                        textEditorService.ModelApi.InsertTextUnsafeFactory(
                                modelModifier.ResourceUri,
                                insertionCursorModifierBag,
                                "    ",
                                CancellationToken.None)
                            .Invoke(editContext);

                        var rowText = new string(
                            modelModifier.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());

                        Assert.Equal("    7 Pillows\n", rowText);
                    }

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(7);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 19;
                    primaryCursorModifier.SelectionEndingPositionIndex = 20;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("P", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentLess.CommandFunc.Invoke(textEditorCommandArgs);

            var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(outModel);

            var rowText = new string(
                outModel!.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());

            Assert.Equal("7 Pillows\n", rowText);
        }

        // Invoke IndexLess on a single line of which the line has one tab character at the start.
        // Select the entire line, including the line ending
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    // Insert a tab character at the start of the row under testing
                    {
                        var cursor = new TextEditorCursor(1, 0, true);
                        var cursorModifier = new TextEditorCursorModifier(cursor);

                        var insertionCursorModifierBag = new CursorModifierBagTextEditor(
                            Key<TextEditorViewModel>.Empty,
                            new List<TextEditorCursorModifier> { cursorModifier });

                        textEditorService.ModelApi.InsertTextUnsafeFactory(
                                modelModifier.ResourceUri,
                                insertionCursorModifierBag,
                                "\t",
                                CancellationToken.None)
                            .Invoke(editContext);

                        var rowText = new string(
                            modelModifier.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());

                        Assert.Equal("\t7 Pillows\n", rowText);
                    }

                    primaryCursorModifier.LineIndex = 1;
                    primaryCursorModifier.SetColumnIndexAndPreferred(0);
                    primaryCursorModifier.SelectionAnchorPositionIndex = 13;
                    primaryCursorModifier.SelectionEndingPositionIndex = 24;

                    // Assert that the text selected, is what was planned
                    {
                        var selectedText = TextEditorSelectionHelper.GetSelectedText(
                            primaryCursorModifier,
                            modelModifier);

                        Assert.Equal("\t7 Pillows\n", selectedText);
                    }

                    return Task.CompletedTask;
                });

            await TextEditorCommandDefaultFacts.IndentLess.CommandFunc.Invoke(textEditorCommandArgs);

            var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(outModel);

            var rowText = new string(
                outModel!.GetLineRichCharacterRange(1, 1).Single().Select(x => x.Value).ToArray());

            Assert.Equal("7 Pillows\n", rowText);
        }
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ClearTextSelection"/>
    /// </summary>
    [Fact]
    public async Task ClearTextSelection()
    {
        // No already existing selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var refViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(refViewModel);
            Assert.Null(refViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.False(TextEditorSelectionHelper.HasSelectedText(refViewModel!.PrimaryCursor.Selection));

            await TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(textEditorCommandArgs);

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);
            Assert.Null(outViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.False(TextEditorSelectionHelper.HasSelectedText(outViewModel!.PrimaryCursor.Selection));
        }

        // With an already existing selection
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    // Select the "Hello" text on the first row
                    {
                        primaryCursorModifier.SelectionAnchorPositionIndex = 0;
                        primaryCursorModifier.SelectionEndingPositionIndex = 5;
                    }

                    // SetColumnIndexAndPreferred is unnecessary, but the user is likely
                    // to have their ColumnIndex == SelectionEndingPositionIndex
                    primaryCursorModifier.SetColumnIndexAndPreferred(
                        primaryCursorModifier.SelectionEndingPositionIndex);

                    return Task.CompletedTask;
                });

            var refViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(refViewModel);
            Assert.NotNull(refViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.True(TextEditorSelectionHelper.HasSelectedText(refViewModel!.PrimaryCursor.Selection));

            await TextEditorCommandDefaultFacts.ClearTextSelection.CommandFunc.Invoke(textEditorCommandArgs);

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);
            Assert.Null(outViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.False(TextEditorSelectionHelper.HasSelectedText(outViewModel!.PrimaryCursor.Selection));
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.NewLineBelow"/>
	/// </summary>
	[Fact]
    public Task NewLineBelow()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //// RowIndex == 0 && ColumnIndex == 0
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is firstRow
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is not firstRow, nor lastRow
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is lastRow
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex == document.RowCount
        ////
        //// That is to say, the final character in the document is a line ending.
        //// Because a cursor can go 1 character further than the document's length,
        //// what happens here?
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at the start of a row
        //// (Note: the cursor is at a column index of 0)
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at neither the start of a row, nor the end of a row
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at the end of a row
        //// (Note: the cursor is immediately before a line ending)
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineBelow.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.NewLineAbove"/>
	/// </summary>
	[Fact]
    public Task NewLineAbove()
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //// RowIndex == 0 && ColumnIndex == 0
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is firstRow
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is not firstRow, nor lastRow
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex is lastRow
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// RowIndex == document.RowCount
        ////
        //// That is to say, the final character in the document is a line ending.
        //// Because a cursor can go 1 character further than the document's length,
        //// what happens here?
        //{

        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at the start of a row
        //// (Note: the cursor is at a column index of 0)
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at neither the start of a row, nor the end of a row
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}

        //// Cursor exists at the end of a row
        //// (Note: the cursor is immediately before a line ending)
        //{
        //    InitializeTextEditorCommandDefaultFactsTests(
        //        out var textEditorService, out var inModel, out var inViewModel,
        //        out var textEditorCommandArgs, out var serviceProvider);

        //    await TextEditorCommandDefaultFacts.NewLineAbove.CommandFunc.Invoke(textEditorCommandArgs);

        //    throw new NotImplementedException();
        //}
    }

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(bool)"/>
	/// </summary>
	[Fact]
	public async Task GoToMatchingCharacterFactory()
	{
        // No shift key
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var newContent = @"namespace BlazorCrudApp.Persons;

public class Person
{
    public Person()
    {
    }
}".ReplaceLineEndings("\n");

            var newRowIndex = 3;
            var newColumnIndex = 0;

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    modelModifier.SetContent(newContent);

                    // Move the cursor to the first curly brace.
                    // In otherwords, the curly brace which opens the 'Person' class.
                    {
                        primaryCursorModifier.LineIndex = newRowIndex;
                        primaryCursorModifier.SetColumnIndexAndPreferred(newColumnIndex);
                    }

                    return Task.CompletedTask;
                });

            var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(refModel);
            Assert.Equal(newContent, refModel!.GetAllText());

            var refViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(refViewModel);
            Assert.Equal(newRowIndex, refViewModel!.PrimaryCursor.LineIndex);
            Assert.Equal(newColumnIndex, refViewModel!.PrimaryCursor.ColumnIndex);
            Assert.Equal(newColumnIndex, refViewModel!.PrimaryCursor.PreferredColumnIndex);

            await TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(false)
                .CommandFunc
                .Invoke(textEditorCommandArgs);

            // The content should NOT have been modified. So Assert that it stayed the same.
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);
                Assert.Equal(newContent, outModel!.GetAllText());
            }

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);
            Assert.Equal(7, outViewModel!.PrimaryCursor.LineIndex);
            Assert.Equal(0, outViewModel!.PrimaryCursor.ColumnIndex);
            Assert.Equal(0, outViewModel!.PrimaryCursor.ColumnIndex);
        }

        // With shift key
        {
            InitializeTextEditorCommandDefaultFactsTests(
                out var textEditorService, out var inModel, out var inViewModel,
                out var textEditorCommandArgs, out var serviceProvider);

            var newContent = @"namespace BlazorCrudApp.Persons;

public class Person
{
    public Person()
    {
    }
}".ReplaceLineEndings("\n");

            var newRowIndex = 3;
            var newColumnIndex = 0;

            textEditorService.PostIndependent(
                nameof(TextEditorCommandDefaultFactsTests),
                editContext =>
                {
                    var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);
                    var viewModelModifier = editContext.GetViewModelModifier(inViewModel.ViewModelKey);
                    var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                    var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                    if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                        return Task.CompletedTask;

                    modelModifier.SetContent(newContent);

                    // Move the cursor to the first curly brace.
                    // In otherwords, the curly brace which opens the 'Person' class.
                    {
                        primaryCursorModifier.LineIndex = newRowIndex;
                        primaryCursorModifier.SetColumnIndexAndPreferred(newColumnIndex);
                    }

                    return Task.CompletedTask;
                });

            var refModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
            Assert.NotNull(refModel);
            Assert.Equal(newContent, refModel!.GetAllText());

            var refViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(refViewModel);
            Assert.Equal(newRowIndex, refViewModel!.PrimaryCursor.LineIndex);
            Assert.Equal(newColumnIndex, refViewModel!.PrimaryCursor.ColumnIndex);
            Assert.Equal(newColumnIndex, refViewModel!.PrimaryCursor.PreferredColumnIndex);

            await TextEditorCommandDefaultFacts.GoToMatchingCharacterFactory(true)
                .CommandFunc
                .Invoke(textEditorCommandArgs);

            // The content should NOT have been modified. So Assert that it stayed the same.
            {
                var outModel = textEditorService.ModelApi.GetOrDefault(inModel.ResourceUri);
                Assert.NotNull(outModel);
                Assert.Equal(newContent, outModel!.GetAllText());
            }

            var outViewModel = textEditorService.ViewModelApi.GetOrDefault(inViewModel.ViewModelKey);
            Assert.NotNull(outViewModel);
            Assert.Equal(7, outViewModel!.PrimaryCursor.LineIndex);
            Assert.Equal(0, outViewModel!.PrimaryCursor.ColumnIndex);
            Assert.Equal(0, outViewModel!.PrimaryCursor.ColumnIndex);
            Assert.Equal(54, outViewModel!.PrimaryCursor.Selection.AnchorPositionIndex);
            Assert.Equal(88, outViewModel!.PrimaryCursor.Selection.EndingPositionIndex);
        }
	}

	/// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.GoToDefinition"/>
	/// </summary>
	[Fact]
    public Task GoToDefinition()
    {
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ShowFindAllDialog"/>
    /// </summary>
    [Fact]
    public Task ShowFindDialog()
    {
        throw new NotImplementedException();
    }

    /// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.Remeasure"/>
	/// </summary>
	[Fact]
    public Task Remeasure()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ShowTooltipByCursorPosition"/>
    /// </summary>
    [Fact]
    public Task ShowTooltipByCursorPosition()
    {
        throw new NotImplementedException();
    }

    /// <summary>
	/// <see cref="TextEditorCommandDefaultFacts.ScrollLineDown"/>
	/// </summary>
	[Fact]
    public Task ScrollLineDown()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ScrollLineUp"/>
    /// </summary>
    [Fact]
    public Task ScrollLineUp()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ScrollPageDown"/>
    /// </summary>
    [Fact]
    public Task ScrollPageDown()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.ScrollPageUp"/>
    /// </summary>
    [Fact]
    public Task ScrollPageUp()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.CursorMovePageBottom"/>
    /// </summary>
    [Fact]
    public Task CursorMovePageBottom()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// <see cref="TextEditorCommandDefaultFacts.CursorMovePageTop"/>
    /// </summary>
    [Fact]
    public Task CursorMovePageTop()
    {
        throw new NotImplementedException();
    }

    private static void InitializeTextEditorCommandDefaultFactsTests(
        out ITextEditorService textEditorService,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out TextEditorCommandArgs textEditorCommandArgs,
        out IServiceProvider serviceProvider)
    {
        throw new NotImplementedException("Test was broken on (2024-04-08)");
        //      var services = new ServiceCollection()
        //          .AddSingleton<LuthetusCommonConfig>()
        //          .AddSingleton<LuthetusTextEditorConfig>()
        //          .AddScoped<IStorageService, DoNothingStorageService>()
        //          .AddScoped<IJSRuntime, TextEditorTestingJsRuntime>()
        //          .AddScoped<StorageSync>()
        //          .AddScoped<IBackgroundTaskService>(_ => new BackgroundTaskServiceSynchronous())
        //          .AddScoped<ITextEditorRegistryWrap, TextEditorRegistryWrap>()
        //          .AddScoped<IDecorationMapperRegistry, DecorationMapperRegistryDefault>()
        //          .AddScoped<ICompilerServiceRegistry, CompilerServiceRegistryDefault>()
        //          .AddScoped<ITextEditorService, TextEditorService>()
        //          .AddScoped<IClipboardService, InMemoryClipboardService>()
        //          .AddFluxor(options => options.ScanAssemblies(
        //              typeof(LuthetusCommonConfig).Assembly,
        //              typeof(LuthetusTextEditorConfig).Assembly));

        //      serviceProvider = services.BuildServiceProvider();

        //      var store = serviceProvider.GetRequiredService<IStore>();
        //      store.InitializeAsync().Wait();

        //      var backgroundTaskService = serviceProvider.GetRequiredService<IBackgroundTaskService>();

        //      var continuousQueue = new BackgroundTaskQueue(
        //          ContinuousBackgroundTaskWorker.GetQueueKey(),
        //          ContinuousBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        //      backgroundTaskService.RegisterQueue(continuousQueue);

        //      var blockingQueue = new BackgroundTaskQueue(
        //          BlockingBackgroundTaskWorker.GetQueueKey(),
        //          BlockingBackgroundTaskWorker.QUEUE_DISPLAY_NAME);

        //      backgroundTaskService.RegisterQueue(blockingQueue);

        //      var textEditorRegistryWrap = serviceProvider.GetRequiredService<ITextEditorRegistryWrap>();

        //      textEditorRegistryWrap.DecorationMapperRegistry = serviceProvider
        //          .GetRequiredService<IDecorationMapperRegistry>();

        //      textEditorRegistryWrap.CompilerServiceRegistry = serviceProvider
        //          .GetRequiredService<ICompilerServiceRegistry>();

        //      textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        //model = new TextEditorModel(
        //          new ResourceUri($"/{nameof(InitializeTextEditorCommandDefaultFactsTests)}.txt"),
        //          DateTime.UtcNow,
        //          ExtensionNoPeriodFacts.TXT,
        //          TestConstants.SOURCE_TEXT,
        //          null,
        //          null);

        //      textEditorService.ModelApi.RegisterCustom(model);

        //      model = textEditorService.ModelApi.GetOrDefault(model.ResourceUri)
        //         ?? throw new ArgumentNullException();

        //      var viewModelKey = Key<TextEditorViewModel>.NewKey();

        //      textEditorService.ViewModelApi.Register(
        //          viewModelKey,
        //          model.ResourceUri,
        //          new TextEditorCategory("UnitTesting"));

        //      viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey)
        //         ?? throw new ArgumentNullException();

        //      textEditorCommandArgs = new TextEditorCommandArgs(
        //          model.ResourceUri,
        //          viewModel.ViewModelKey,
        //          false,
        //          serviceProvider.GetRequiredService<IClipboardService>(),
        //          textEditorService,
        //          (MouseEventArgs m) => Task.CompletedTask,
        //          serviceProvider.GetRequiredService<IJSRuntime>(),
        //          serviceProvider.GetRequiredService<IDispatcher>(),
        //          serviceProvider.GetRequiredService<IServiceProvider>(),
        //          serviceProvider.GetRequiredService<LuthetusTextEditorConfig>());
    }
}