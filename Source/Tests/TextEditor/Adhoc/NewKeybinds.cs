using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class NewKeybinds
{
	/// <summary>
	/// [] Alt + Up => move line up
	/// 	[] 2 ways about this?
	/// 		[] Way One
	/// 			[] Determine the start position index of the current line.
	/// 			[] Cut the line above
	/// 			[] Paste the contents at the determined start position index.
	/// 		[] Way Two
	/// 			[] Determine the start position index of the previous line.
	/// 			[] Cut the current line
	/// 			[] Paste the contents at the determined start position index
	/// </summary>
	[Fact]
	public void AltPlusUp_THEN_MoveLineUp()
	{
		var initialContent = @"abc
123
doremi".ReplaceLineEndings("\n");

		var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            initialContent,
            null,
            null);
            
		Assert.Equal(initialContent, model.GetAllText());
            
        var modelModifier = new TextEditorModelModifier(model);
        KeybindUnderTest();

        var expectedtext = @"123
abc
doremi".ReplaceLineEndings("\n");

        var actualText = modelModifier.GetAllText();
        
        Assert.Equal(expectedtext, actualText);
		
		// Going to write the keybind here then move this code when done
		void KeybindUnderTest()
		{
			var cursor = new TextEditorCursor(
				lineIndex: 1,
				columnIndex: 0,
				isPrimaryCursor: true);
				
			var previousLineIndex = cursor.LineIndex - 1;
			var previousLineInformation = modelModifier.GetLineInformation(previousLineIndex);

			// Insert
			{
				var cursorModifier = new TextEditorCursorModifier(cursor);
				cursorModifier.LineIndex = previousLineIndex;
				cursorModifier.ColumnIndex = 0;
				
				var currentLineContent = modelModifier.GetLineTextRange(cursor.LineIndex, 1);
	
				var cursorModifierBag = new CursorModifierBagTextEditor(
			        Key<TextEditorViewModel>.Empty,
			        new List<TextEditorCursorModifier> { cursorModifier });
	
				modelModifier.Insert(
					value: currentLineContent,
					cursorModifierBag: cursorModifierBag,
					useLineEndKindPreference: false);
			}

			// Delete
			{
				var cursorModifier = new TextEditorCursorModifier(cursor);
				
				// Add 1 because a line was inserted
				cursorModifier.LineIndex = cursor.LineIndex + 1;
				cursorModifier.ColumnIndex = 0;
				
				var currentLineInformation = modelModifier.GetLineInformation(cursorModifier.LineIndex);
				var columnCount = currentLineInformation.EndPositionIndexExclusive -
					currentLineInformation.StartPositionIndexInclusive;
	
				var cursorModifierBag = new CursorModifierBagTextEditor(
			        Key<TextEditorViewModel>.Empty,
			        new List<TextEditorCursorModifier> { cursorModifier });
	
				modelModifier.Delete(
			        cursorModifierBag,
			        columnCount,
			        false,
			        TextEditorModelModifier.DeleteKind.Delete);
			}
			
			Console.WriteLine(modelModifier.GetAllText());
		}
	}

	/// <summary>
	/// [] Alt + Down => move line down
	/// 	[] see the other version of this
	/// </summary>
	[Fact]
	public void AltPlusDown_THEN_MoveLineDown()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] Ctrl + Alt + (ArrowLeft | ArrowRight) => move camel case
	/// 	[] while (LetterOrDigit) if IsCapitalized break;
	/// </summary>
	[Fact]
	public void CtrlPlusAltPlusArrowLeftOrArrowRight_THEN_MoveCamelCase()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] Home keystroke
	/// 	[] Should "stop early" at the end of the indentation.
	/// 		[] Go to column 0
	/// 		[] While (indentation) ++column
	/// 	[] Then repressing the keystroke at the end of the indentation goes to the actual start of the line
	/// 		[] Track current column
	/// 		[] Do the "stop early" logic
	/// 		[] Compare if previous column and new current column are the same
	/// 		[] if the same then go to column 0
	/// 		[] otherwise stay where you are.
	/// 	[] If at start of line then home takes to end of indentation.
	/// 		[] Do the "stop early" logic
	/// </summary>
	[Fact]
	public void Home_THEN_MoreFunctionality()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] Match indentation on newline
	/// 	[] insert line above command
	/// 		[] The code to get the indentation is already done in the 'HandleOnKeyDown(...)' method,
	/// 		   	note: I totally spelled the method name wrong.
	/// </summary>
	[Fact]
	public void CtrlPlusEnter_THEN_SupportMatchIndentation()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] Match indentation on newline
	/// 	[] insert line below command
	/// 		[] The code to get the indentation is already done in the 'HandleOnKeyDown(...)' method,
	///            	note: I totally spelled the method name wrong.
	/// </summary>
	[Fact]
	public void ShiftPlusEnter_THEN_SupportMatchIndentation()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] No selection + { Shift + Tab } + Cursor positioned at or in the starting indentation should => IndentLess
	/// 	[] Go to column 0
	/// 		[] While (indentation) ++column
	/// 			[] if pass by the cursor, then can break early with success
	/// 			[] if sees !indentation then break with failure
	/// </summary>
	[Fact]
	public void NoSelectionPlusShiftPlusTabPlusCursorAtIndentation_THEN_IndentLess()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] F7 => Related Files quickpick
	/// 	[] Read directory which contains the current file
	/// 	[] If there are any direct descendent files of that directory then check the name against a
	///        	predicate if it "matches" with the active file and add to list if it does
	/// 	[] Show the list to the user
	/// 	[] Open the file they pick
	/// 	[] Or let them cancel with 'Escape' key
	/// </summary>
	[Fact]
	public void F7_THEN_RelatedFilesQuickpick()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// [] Ctrl + Tab => Quick Pick file or context/tool
	/// 	[] I can probably make this task first draft far simpler by not closing the menu on-ctrl-key-up
	/// 	[] Visual Studio has where you hold ctrl, but it sounds like a pain to do and it makes me want to procrastinate.
	/// 	[] If I just have Ctrl + Tab open the menu then either enter to select and submit form or escape to close menu then
	///        	it would be a lot easier.
	/// 	[] I say this because I imagine I need something stateful in order to have on-ctrl-key-up logic work.
	///        	I have to maintain the state of this and subscribe to the event in a sort of way.
	/// </summary>
	[Fact]
	public void CtrlPlusTab_THEN_FileOrContextOrToolQuickpick()
	{
		throw new NotImplementedException();
	}
}
