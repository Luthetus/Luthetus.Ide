using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class NewKeybinds
{
	/// <summary>
	/// [] Alt + Down => move line down
	/// 	[] see the other version of this
	/// </summary>
	[Fact]
	public void AltPlusDown_THEN_MoveLineDown()
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
				lineIndex: 0,
				columnIndex: 0,
				isPrimaryCursor: true);
				
			var nextLineIndex = cursor.LineIndex + 1;
			var nextLineInformation = modelModifier.GetLineInformation(nextLineIndex);

			// Insert
			{
				var cursorModifier = new TextEditorCursorModifier(cursor);
				cursorModifier.LineIndex = nextLineIndex + 1;
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
				
				cursorModifier.LineIndex = cursor.LineIndex;
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
	/// [] Ctrl + Alt + (ArrowLeft | ArrowRight) => move camel case
	/// 	[] while (LetterOrDigit) if IsCapitalized break;
	/// </summary>
	[Fact]
	public void CtrlPlusAltPlusArrowLeftOrArrowRight_THEN_MoveCamelCase()
	{
		var initialContent = @"AppleBananaCucumber";

		var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            initialContent,
            null,
            null);
            
		Assert.Equal(initialContent, model.GetAllText());
            
        var modelModifier = new TextEditorModelModifier(model);
        
        var cursor = new TextEditorCursor(
			lineIndex: 0,
			columnIndex: 0,
			isPrimaryCursor: true);

        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(0, cursor.ColumnIndex);
        Assert.Equal(0, cursor.PreferredColumnIndex);
        
        KeybindUnderTest();
        
        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(5, cursor.ColumnIndex);
        Assert.Equal(5, cursor.PreferredColumnIndex);

		throw new NotImplementedException("TODO: Handle edge cases");
		
		// Going to write the keybind here then move this code when done
		void KeybindUnderTest()
		{
			var positionIndex = modelModifier.GetPositionIndex(cursor);
			var rememberStartPositionIndex = positionIndex;
			
			var startCharacterKind = CharacterKindHelper.CharToCharacterKind(
				model.RichCharacterList[positionIndex].Value);
				
			if (startCharacterKind != CharacterKind.LetterOrDigit)
			{
				throw new NotImplementedException("Invoke GetColumnIndexOfCharacterWithDifferingKind instead");
			}
				
			while (++positionIndex < modelModifier.RichCharacterList.Count)
			{
				var currentRichCharacter = modelModifier.RichCharacterList[positionIndex];
				
				var currentCharacterKind = CharacterKindHelper.CharToCharacterKind(
					currentRichCharacter.Value);
				
				if (currentCharacterKind != CharacterKind.LetterOrDigit)
					break;
				if (Char.IsUpper(currentRichCharacter.Value))
					break;
			}
			
			var columnDisplacement = positionIndex - rememberStartPositionIndex;
			
			var cursorModifier = new TextEditorCursorModifier(cursor);
			cursorModifier.SetColumnIndexAndPreferred(cursor.ColumnIndex + columnDisplacement);

			cursor = cursorModifier.ToCursor();
		}
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
	/// 	[] If within starting indentation then go to the end of the starting indentation.
	/// 		[] Do the "stop early" logic
	/// </summary>
	[Fact]
	public void Home_THEN_MoreFunctionality()
	{
		var initialContent = @"	AppleBananaCucumber";

		var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            initialContent,
            null,
            null);
            
		Assert.Equal(initialContent, model.GetAllText());
            
        var modelModifier = new TextEditorModelModifier(model);
        
        var lineInformation = modelModifier.GetLineInformation(0);
        var cursor = new TextEditorCursor(
        	lineIndex: 0,
        	columnIndex: lineInformation.LastValidColumnIndex,
        	isPrimaryCursor: true);
        	
        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(lineInformation.LastValidColumnIndex, cursor.ColumnIndex);
        Assert.Equal(lineInformation.LastValidColumnIndex, cursor.PreferredColumnIndex);
        
        KeybindUnderTest();

        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(1, cursor.ColumnIndex);
        Assert.Equal(1, cursor.PreferredColumnIndex);
        
		throw new NotImplementedException("TODO: Handle edge cases");
		
		// Going to write the keybind here then move this code when done
		void KeybindUnderTest()
		{
			var cursorModifier = new TextEditorCursorModifier(cursor);
			var originalPositionIndex = modelModifier.GetPositionIndex(cursorModifier);
			
			cursorModifier.ColumnIndex = 0;
			
			var lineInformation = modelModifier.GetLineInformation(cursorModifier.LineIndex);
			var indentationPositionIndex = modelModifier.GetPositionIndex(cursorModifier);
			
			var cursorWithinIndentation = false;

			while (indentationPositionIndex < lineInformation.LastValidColumnIndex)
			{
				var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndex++].Value;

				if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
				{
					if (indentationPositionIndex == originalPositionIndex)
					{
						cursorWithinIndentation = true;
						break;
					}
				}
				else
				{
					// Eager incrementation is making this too large by 1
					indentationPositionIndex--;
					break;
				}
			}
			
			if (originalPositionIndex == 0)
			{
				var exclusiveEndIndentationPositionIndex = indentationPositionIndex;
				cursorModifier.SetColumnIndexAndPreferred(exclusiveEndIndentationPositionIndex);
			}
			else if (!cursorWithinIndentation)
			{
				var exclusiveEndIndentationPositionIndex = indentationPositionIndex;
				cursorModifier.SetColumnIndexAndPreferred(exclusiveEndIndentationPositionIndex);
			}
			// else: Stay at column 0
			
			cursor = cursorModifier.ToCursor();
		}
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
		throw new NotImplementedException("Made the changes directly, it seems to work but needs testing");
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
		throw new NotImplementedException("Made the changes directly, it seems to work but needs testing");
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
		var initialContent = @"	AppleBananaCucumber";

		var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            initialContent,
            null,
            null);
            
		Assert.Equal(initialContent, model.GetAllText());
            
        var modelModifier = new TextEditorModelModifier(model);
        
        var cursor = new TextEditorCursor(
        	lineIndex: 0,
        	columnIndex: 0,
        	isPrimaryCursor: true);
        	
        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(0, cursor.ColumnIndex);
        Assert.Equal(0, cursor.PreferredColumnIndex);
        
        var initialContentLength = initialContent.Length;
        
        KeybindUnderTest();

        Assert.Equal(0, cursor.LineIndex);
        Assert.Equal(0, cursor.ColumnIndex);
        Assert.Equal(0, cursor.PreferredColumnIndex);
        
        var endContentLength = modelModifier.GetAllText().Length;
        
        // One tab key was removed, therefore assert a length smaller by 1
        Assert.Equal(initialContentLength - 1, endContentLength);
        
		throw new NotImplementedException();
		
		// Going to write the keybind here then move this code when done
		void KeybindUnderTest()
		{
			var cursorModifier = new TextEditorCursorModifier(cursor);
			var originalPositionIndex = modelModifier.GetPositionIndex(cursorModifier);
			
			cursorModifier.ColumnIndex = 0;
			
			var lineInformation = modelModifier.GetLineInformation(cursorModifier.LineIndex);
			var indentationPositionIndex = modelModifier.GetPositionIndex(cursorModifier);
			
			var cursorWithinIndentation = false;

			while (indentationPositionIndex < lineInformation.LastValidColumnIndex)
			{
				var possibleIndentationChar = modelModifier.RichCharacterList[indentationPositionIndex++].Value;
				
				if (possibleIndentationChar == '\t' || possibleIndentationChar == ' ')
				{
					if (indentationPositionIndex == originalPositionIndex)
					{
						cursorWithinIndentation = true;
						break;
					}
				}
				else
				{
					// Eager incrementation is making this too large by 1
					indentationPositionIndex--;
					break;
				}
			}
			
			if (originalPositionIndex == 0)
			{
				var exclusiveEndIndentationPositionIndex = indentationPositionIndex;
				cursorModifier.SetColumnIndexAndPreferred(exclusiveEndIndentationPositionIndex);
			}
			else if (!cursorWithinIndentation)
			{
				var exclusiveEndIndentationPositionIndex = indentationPositionIndex;
				cursorModifier.SetColumnIndexAndPreferred(exclusiveEndIndentationPositionIndex);
			}
			// else: Stay at column 0
			
			cursor = cursorModifier.ToCursor();
		}
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
