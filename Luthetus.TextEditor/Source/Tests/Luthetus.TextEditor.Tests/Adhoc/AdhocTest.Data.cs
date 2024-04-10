namespace Luthetus.TextEditor.Tests.Adhoc;

public partial class AdhocTest
{
    /// <summary>
    /// <see cref="RazorLib.TextEditors.Models.TextEditorModels.ITextEditorModel.ContentList"/>
    /// <summary>
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
    ///             -SampleText: public class MyClass { }
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
    /// </summary>
    public const string _contentListChangeSampleText = @"";
}
