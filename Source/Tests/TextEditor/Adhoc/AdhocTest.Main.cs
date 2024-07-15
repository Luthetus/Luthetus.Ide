using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.CompilerServices.Models;
using Luthetus.Ide.RazorLib.Decorations;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class AdhocTest
{
    [Fact]
    public void Why_Is_Command_Cut_Very_Slow()
    {
        var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            _bigString,
            null,
            null);

        var modelModifier = new TextEditorModelModifier(model);

        var beforeAllText = modelModifier.GetAllText();

        modelModifier.__RemoveRange(0, 5);

        var afterAllText = modelModifier.GetAllText();

        modelModifier.__RemoveRange(0, 5);

        var anotherAfterAllText = modelModifier.GetAllText();
    }

    [Fact]
    public void Insert_Into_Empty_Model_A_LetterOrDigit()
    {
        var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            string.Empty,
            null,
            null);

        var modelModifier = new TextEditorModelModifier(model);

        // Before-assertions
        {
            Assert.Empty(model.RichCharacterList);
            //
            Assert.Equal(1, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Single();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(0, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(0, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }

        // Variables
        var letterToInsert = "a";

        // State change
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);
            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(letterToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);
            model = modelModifier.ToModel();
        }

        // After-assertions
        {
            Assert.Single(model.RichCharacterList);
            Assert.Equal(letterToInsert, model.RichCharacterList.Single().Value.ToString());
            //
            Assert.Equal(1, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Single();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(1, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(1, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }
    }

    [Fact]
    public void Insert_Into_Empty_Model_A_LineFeed()
    {
        var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            string.Empty,
            null,
            null);

        var modelModifier = new TextEditorModelModifier(model);

        // Before-assertions
        {
            Assert.Empty(model.RichCharacterList);
            //
            Assert.Equal(1, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Single();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(0, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(0, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }

        // Variables
        var letterToInsert = "\n";

        // State change
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);
            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(letterToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);
            model = modelModifier.ToModel();
        }

        // After-assertions
        {
            Assert.Single(model.RichCharacterList);
            Assert.Equal(letterToInsert, model.RichCharacterList.Single().Value.ToString());
            //
            Assert.Equal(2, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Last();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(1, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(1, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(1, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }
    }

    [Fact]
    public void Insert_Into_Empty_Model_A_CarriageReturn()
    {
        var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            string.Empty,
            null,
            null);

        var modelModifier = new TextEditorModelModifier(model);

        // Before-assertions
        {
            Assert.Empty(model.RichCharacterList);
            //
            Assert.Equal(1, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Single();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(0, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(0, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }

        // Variables
        var letterToInsert = "\r";
        //# Any CarriageReturnNewLine, or CarriageReturn, is currently
        //# replaced with '\n'.
        //#
        //# Therefore the expected letter that was inserted would be "\n"
        //# (2024-04-12)
        var expectedLetterThatWasInserted = "\n";

        // State change
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);
            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(letterToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);
            model = modelModifier.ToModel();
        }

        // After-assertions
        {
            Assert.Single(model.RichCharacterList);
            Assert.Equal(expectedLetterThatWasInserted, model.RichCharacterList.Single().Value.ToString());
            //
            Assert.Equal(2, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Last();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(1, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(1, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(1, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }
    }

    [Fact]
    public void Insert_Into_Empty_Model_A_CarriageReturnNewLine()
    {
        var model = new TextEditorModel(
            new ResourceUri("/unitTesting.cs"),
            DateTime.UtcNow,
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
            string.Empty,
            null,
            null);

        var modelModifier = new TextEditorModelModifier(model);

        // Before-assertions
        {
            Assert.Empty(model.RichCharacterList);
            //
            Assert.Equal(1, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Last();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(0, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(0, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }

        // Variables
        var letterToInsert = "\r\n";
        //# Any CarriageReturnNewLine, or CarriageReturn, is currently
        //# replaced with '\n'.
        //#
        //# Therefore the expected letter that was inserted would be "\n"
        //# (2024-04-12)
        var expectedLetterThatWasInserted = "\n";

        // State change
        {
            var cursor = new TextEditorCursor(
                lineIndex: 0,
                columnIndex: 0,
                isPrimaryCursor: true);
            var cursorModifier = new TextEditorCursorModifier(cursor);
            var cursorModifierBag = new CursorModifierBagTextEditor(
                Key<TextEditorViewModel>.Empty,
                new List<TextEditorCursorModifier>() { cursorModifier });

            modelModifier.Insert(letterToInsert, cursorModifierBag, cancellationToken: CancellationToken.None);
            model = modelModifier.ToModel();
        }

        // After-assertions
        {
            Assert.Single(model.RichCharacterList);
            Assert.Equal(expectedLetterThatWasInserted, model.RichCharacterList.Single().Value.ToString());
            //
            Assert.Equal(2, model.LineCount);
            var endOfFileLineEnd = model.LineEndList.Last();
            Assert.Equal(LineEndKind.EndOfFile, endOfFileLineEnd.LineEndKind);
            Assert.Equal(1, endOfFileLineEnd.StartPositionIndexInclusive);
            Assert.Equal(1, endOfFileLineEnd.EndPositionIndexExclusive);
            //
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturn).count);
            Assert.Equal(1, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.LineFeed).count);
            Assert.Equal(0, model.LineEndKindCountList.Single(x => x.lineEndKind == LineEndKind.CarriageReturnLineFeed).count);
            //
            Assert.Empty(model.TabKeyPositionList);
        }
    }

    /// <summary>
    /// <see cref="ITextEditorModel.ContentList"/><br/><br/>
    /// TODO: This needs to be separated out into an IReadOnlyList&lt;char&gt;...
    ///       ...and a IReadOnlyList&lt;byte&gt;
    ///       |
    ///       This change is needed because, a large part of optimizing the text editor is
    ///       to reduce the operations done on the entirety of the text editor.
    ///       |
    ///       And, currently one invokes textEditorModel.GetAllText() rather frequently, and redundantly.
    ///       |
    ///       This invocation of 'GetAllText()' needs to iterate over every character in the text editor,
    ///       and select its 'Value' property.
    ///       |
    ///       This is an operation done on the entirety of the text editor, and therefore
    ///       it should be optimized.
    ///       |
    ///       The result of the change will make 'GetAllText()' simply return a reference
    ///       to the underlying list of characters.
    /// =================================================================================================
    /// What might break with this change?:
    ///     -Decoration, more specifically syntax highlighting, could break with this change.
    ///         -Could I, prior to making the change, write a unit test that applies syntax highlighting
    ///             to a simple C# class definition. The, run this test throughout the time I'm making the change,
    ///             and afterwards to ensure that syntax highlighting was (seemingly) not broken?
    ///             -SampleText: "public class MyClass { }"
    ///             -Determine what the Lexer's syntax tokens are after the 'Lex()' method invocation.
    ///                  As well, dermine result of the Parser, CompilationUnit, and CSharpResource.
    ///                  -Save the data so it can be compared against for future tests,
    ///                       to see if the result erroneously changed.
    ///     -Regarding 'GetAllText()' method on 'ITextEditorModel'.
    ///         -What will become of the 'GetAllText()' method after these changes?
    ///     -Regarding 'PartitionList' property on 'ITextEditorModel'.
    ///         -This is currently an 'ImmutableList&lt;ImmutableList&lt;RichCharacter&gt;&gt;'
    ///             -This would need be changed to an 'ImmutableList&lt;ImmutableList&lt;char&gt;&gt;'
    ///                 -NOTE: The 'PartitionList' is supposed to encompase all the metadata for the text
    ///                      within that partition.
    ///                      -This is quite difficult, so to start, I moved the RichCharacters to be partitioned.
    ///                           and am "globally" tracking the metadata.
    ///                      -The metadata meaning:
    ///                           -RowEndingPositionsList
    ///                           -RowEndingKindCountsList
    ///                           -TabKeyPositionsList
    ///                           -OnlyRowEndingKind
    ///                           -UsingRowEndingKind
    ///                           -EditBlockIndex
    ///                           -MostCharactersOnASingleRowTuple
    ///                           -RowCount
    ///                           -PartitionLength
    ///                           -NOTE: Should 'PresentationModelsList' be part of a partition?
    ///                                -If one were to have a sufficiently-large file C# file.
    ///                                     Then there might be an overwhelming amount of "squigglies" (diagnostics),
    ///                                         if they were stored "globally".
    ///     -How would one lookup the decoration byte for a given character after this change?
    ///         -The 'CharDecorationByteList' would have an index which is 1 to 1 with the character's index in
    ///              the list of characters.
    ///         -Could implicit conversions between byte and char (or vice versa) cause hard to debug confusion?
    ///              -The implicit conversion of byte and char (and vice versa) should be checked.
    ///                   -A char is 2 bytes in C#, so it isn't believed that any implicit conversion
    ///                        would be done (vice versa).
    ///     -What properties will replace the 'IReadOnlyList&lt;RichCharacter&gt; ContentList' property?
    ///         -IReadOnlyList&lt;char&gt; CharList
    ///         -IReadOnlyList&lt;byte&gt; DecorationByteList
    /// </summary>
    [Fact]
    public void ContentList_Change()
    {
        InitializeTextEditorServicesTestsHelper(
            out var initialContent,
            out var refModel,
            out var viewModel,
            out var textEditorService);

        Assert.Equal(initialContent, refModel.GetAllText());

        refModel.CompilerService.ResourceWasModified(
            refModel.ResourceUri,
            ImmutableArray<TextEditorTextSpan>.Empty);

        refModel = textEditorService.ModelApi.GetOrDefault(refModel.ResourceUri) ?? throw new ArgumentNullException();

        textEditorService.PostDistinct(
            nameof(ContentList_Change),
            textEditorService.ModelApi.ApplySyntaxHighlightingFactory(refModel.ResourceUri));

        // ContentList
        {
            var contentList = refModel.RichCharacterList;
            Assert.Equal(27, refModel.RichCharacterList.Count);

            var i = 0;

            // Idea: Would a method, 'RichCharacter GetRichCharacter(int globalPositionIndex)', be good?
            //       Currently, I have to index into two separate lists, i.e. the 'CharList'
            //       and the DecorationByteList.
            //       |
            //       Internally 'RichCharacter GetRichCharacter(int globalPositionIndex)' would be no different,
            //       but perhaps the API would provide some sanity?

            var richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('p', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('u', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('b', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('l', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('i', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('c', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal(' ', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('c', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('l', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('a', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('s', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('s', richCharacter.Value);
            Assert.Equal(1, richCharacter.DecorationByte);
            
            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal(' ', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('M', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('y', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('C', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('l', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('a', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('s', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('s', richCharacter.Value);
            Assert.Equal(11, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('\n', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('{', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('\n', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('\t', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('\n', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('}', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            richCharacter = refModel.RichCharacterList[i++] ?? throw new ArgumentNullException();
            Assert.Equal('\n', richCharacter.Value);
            Assert.Equal(0, richCharacter.DecorationByte);

            Assert.Equal(27, i);
        }

        // PartitionList
        {
            var partitionList = refModel.PartitionList;
            Assert.Single(partitionList);
        }

        // EditBlocksList
        {
            var editBlocksList = refModel.EditBlockList;
            Assert.Empty(editBlocksList);
        }

        // RowEndingPositionsList
        {
            var rowEndingPositionsList = refModel.LineEndList;

            Assert.Equal(5, rowEndingPositionsList.Count);

            var i = 0;

            var rowEnding = rowEndingPositionsList[i++];
            Assert.Equal(20, rowEnding.StartPositionIndexInclusive);
            Assert.Equal(21, rowEnding.EndPositionIndexExclusive);
            Assert.Equal(LineEndKind.LineFeed, rowEnding.LineEndKind);

            rowEnding = rowEndingPositionsList[i++];
            Assert.Equal(22, rowEnding.StartPositionIndexInclusive);
            Assert.Equal(23, rowEnding.EndPositionIndexExclusive);
            Assert.Equal(LineEndKind.LineFeed, rowEnding.LineEndKind);

            rowEnding = rowEndingPositionsList[i++];
            Assert.Equal(24, rowEnding.StartPositionIndexInclusive);
            Assert.Equal(25, rowEnding.EndPositionIndexExclusive);
            Assert.Equal(LineEndKind.LineFeed, rowEnding.LineEndKind);

            rowEnding = rowEndingPositionsList[i++];
            Assert.Equal(26, rowEnding.StartPositionIndexInclusive);
            Assert.Equal(27, rowEnding.EndPositionIndexExclusive);
            Assert.Equal(LineEndKind.LineFeed, rowEnding.LineEndKind);

            rowEnding = rowEndingPositionsList[i++];
            Assert.Equal(27, rowEnding.StartPositionIndexInclusive);
            Assert.Equal(27, rowEnding.EndPositionIndexExclusive);
            Assert.Equal(LineEndKind.EndOfFile, rowEnding.LineEndKind);
        }

        // RowEndingKindCountsList
        {
            var rowEndingKindCountsList = refModel.LineEndKindCountList;

            Assert.Equal(3, rowEndingKindCountsList.Count);

            var i = 0;

            var rowEndingKindCountTuple = rowEndingKindCountsList[i++];
            Assert.Equal(LineEndKind.CarriageReturn, rowEndingKindCountTuple.lineEndKind);
            Assert.Equal(0, rowEndingKindCountTuple.count);

            rowEndingKindCountTuple = rowEndingKindCountsList[i++];
            Assert.Equal(LineEndKind.LineFeed, rowEndingKindCountTuple.lineEndKind);
            Assert.Equal(4, rowEndingKindCountTuple.count);

            rowEndingKindCountTuple = rowEndingKindCountsList[i++];
            Assert.Equal(LineEndKind.CarriageReturnLineFeed, rowEndingKindCountTuple.lineEndKind);
            Assert.Equal(0, rowEndingKindCountTuple.count);
        }

        // PresentationModelsList
        {
            var presentationModelsList = refModel.PresentationModelList;
            Assert.Single(presentationModelsList);
        }

        // TabKeyPositionsList
        {
            var tabKeyPositionsList = refModel.TabKeyPositionList;

            Assert.Single(tabKeyPositionsList);

            var tabKeyPosition = tabKeyPositionsList.Single();
            Assert.Equal(23, tabKeyPosition);
        }

        // OnlyRowEndingKind
        {
            var onlyRowEndingKind = refModel.OnlyLineEndKind;
            Assert.Equal(LineEndKind.LineFeed, onlyRowEndingKind);
        }

        // UsingRowEndingKind
        {
            var usingRowEndingKind = refModel.LineEndKindPreference;
            Assert.Equal(LineEndKind.LineFeed, usingRowEndingKind);
        }

        // ResourceUri
        {
            var resourceUri = refModel.ResourceUri;
            Assert.Equal(new ResourceUri("/unitTesting.cs"), resourceUri);
        }

        // ResourceLastWriteTime
        {
            // Skip
        }

        // PartitionSize
        {
            var partitionSize = refModel.PartitionSize;
            Assert.Equal(4096, partitionSize);
        }

        // FileExtension
        {
            var fileExtension = refModel.FileExtension;
            Assert.Equal(ExtensionNoPeriodFacts.C_SHARP_CLASS, fileExtension);
        }

        // DecorationMapper
        {
            var decorationMapper = refModel.DecorationMapper;
            Assert.IsType<GenericDecorationMapper>(decorationMapper);
        }

        // CompilerService
        {
            var compilerService = refModel.CompilerService;
            Assert.IsType<CSharpCompilerService>(compilerService);
        }

        // TextEditorSaveFileHelper
        {
            var textEditorSaveFileHelper = refModel.TextEditorSaveFileHelper;
            Assert.NotNull(textEditorSaveFileHelper);
        }

        // EditBlockIndex
        {
            var editBlockIndex = refModel.EditBlockIndex;
            Assert.Equal(0, editBlockIndex);
        }

        // IsDirty
        {
            var isDirty = refModel.IsDirty;
            Assert.False(isDirty);
        }

        // MostCharactersOnASingleRowTuple
        {
            var mostCharactersOnASingleLineTuple = refModel.MostCharactersOnASingleLineTuple;
            Assert.Equal(0, mostCharactersOnASingleLineTuple.lineIndex);
            Assert.Equal(26, mostCharactersOnASingleLineTuple.lineLength);
        }

        // RenderStateKey
        {
            // Skip
        }

        // LineCount
        {
            var lineCount = refModel.LineCount;
            Assert.Equal(5, lineCount);
        }

        // DocumentLength
        {
            var documentLength = refModel.DocumentLength;
            Assert.Equal(27, documentLength);
        }

        // cSharpResource
        {
            var cSharpResource = refModel.CompilerService.GetCompilerServiceResourceFor(refModel.ResourceUri) ?? throw new ArgumentNullException();

            // CompilationUnit
            {
                // Skip
            }

            // SyntaxTokenList
            {
                var syntaxTokenList = cSharpResource.SyntaxTokenList;

                Assert.Equal(6, syntaxTokenList.Length);

                var i = 0;

                var token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.PublicTokenKeyword, token.SyntaxKind);
                Assert.Equal("public", token.TextSpan.GetText());

                token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.ClassTokenKeyword, token.SyntaxKind);
                Assert.Equal("class", token.TextSpan.GetText());

                token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.IdentifierToken, token.SyntaxKind);
                Assert.Equal("MyClass", token.TextSpan.GetText());

                token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.OpenBraceToken, token.SyntaxKind);
                Assert.Equal("{", token.TextSpan.GetText());

                token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.CloseBraceToken, token.SyntaxKind);
                Assert.Equal("}", token.TextSpan.GetText());

                token = syntaxTokenList[i++];
                Assert.Equal(SyntaxKind.EndOfFileToken, token.SyntaxKind);
                Assert.Equal(string.Empty, token.TextSpan.GetText());

                Assert.Equal(6, i);
            }

            // GetTokenTextSpans()
            {
                // Skip
            }

            // GetSymbols()
            {
                var symbolList = cSharpResource.GetSymbols();

                Assert.Single(symbolList);

                var symbol = symbolList.Single();
                Assert.Equal(SyntaxKind.TypeSymbol, symbol.SyntaxKind);
                Assert.Equal("MyClass", symbol.TextSpan.GetText());
            }

            // GetDiagnostics()
            {
                var diagnostics = cSharpResource.GetDiagnostics();
                Assert.Empty(diagnostics);
            }
        }
    }

    private static void InitializeTextEditorServicesTestsHelper(
        out string initialContent,
        out TextEditorModel model,
        out TextEditorViewModel viewModel,
        out ITextEditorService textEditorService)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();

        var services = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusIdeRazorLibServices(
                new LuthetusHostingInformation(LuthetusHostingKind.UnitTestingSynchronous, backgroundTaskService));

        var serviceProvider = services.BuildServiceProvider();
        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var textEditorRegistryWrap = serviceProvider.GetRequiredService<ITextEditorRegistryWrap>();
        textEditorRegistryWrap.DecorationMapperRegistry = serviceProvider.GetRequiredService<IDecorationMapperRegistry>();
        textEditorRegistryWrap.CompilerServiceRegistry = serviceProvider.GetRequiredService<ICompilerServiceRegistry>();

        textEditorService = serviceProvider.GetRequiredService<ITextEditorService>();

        var fileExtension = ExtensionNoPeriodFacts.C_SHARP_CLASS;
        var resourceUri = new ResourceUri("/unitTesting.cs");
        var resourceLastWriteTime = DateTime.UtcNow;
        initialContent = "public class MyClass\n{\n\t\n}\n".ReplaceLineEndings("\n");
        var genericDecorationMapper = ((DecorationMapperRegistry)serviceProvider.GetRequiredService<IDecorationMapperRegistry>()).GenericDecorationMapper;
        var cSharpCompilerService = ((CompilerServiceRegistry)serviceProvider.GetRequiredService<ICompilerServiceRegistry>()).CSharpCompilerService;

        model = new TextEditorModel(
             resourceUri,
             resourceLastWriteTime,
             fileExtension,
             initialContent,
             genericDecorationMapper,
             cSharpCompilerService);

        textEditorService.ModelApi.RegisterCustom(model);
        cSharpCompilerService.RegisterResource(model.ResourceUri);
        model = textEditorService.ModelApi.GetOrDefault(resourceUri) ?? throw new ArgumentNullException();

        var viewModelKey = Key<TextEditorViewModel>.NewKey();
        textEditorService.ViewModelApi.Register(viewModelKey, resourceUri, new Category("UnitTesting"));
        viewModel = textEditorService.ViewModelApi.GetOrDefault(viewModelKey) ?? throw new ArgumentNullException();
    }
}
