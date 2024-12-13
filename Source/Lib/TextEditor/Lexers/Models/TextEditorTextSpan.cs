using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

/// <summary>
/// TODO: I have a suspicion that this type takes an absurd amount of memory...
///       ... the initial way of implementing this type was to store the text
///       at the time of constructing the text span.
///       |
///       But, it was thought that preferably one would only only
///       create the substring when asked for it.
///       |
///       This means one has to store the entirety of the source text.
///       Well, the source text is just a reference not a copy of the value.
///       So this seemed fine enough.
///       |
///       But, because the text editor is immutable. The source text
///       is constantly needing to be reconstructed as a string value.
///       |
///       As a result, the old string values that pertain to previous
///       iterations of the text editor must be held in memory,
///       because any TextEditorTextSpan that is not garbage collected,
///       would have a reference to the old string value.
///       |
///       I'm thinking of using Span<char> or something along the lines.
///       But, wouldn't Span<char> still need to maintain the previous string
///       in its entirety?
///       |
///       If Span<T> will somehow track what section of the previous string I
///       have a reference to, and only free the resources pertaining to the
///       sections of the string that don't have a Span<T> referencing them
///       then that would be amazing.
///       |
///       But then again, why am I not just letting the text span capture the
///       substring upon construction?
///       |
///       I also wonder, what about a Func<string>? Does this somehow
///       hide the previous string values of the text editor and
///       permit the resources to be free'd?
///       (2024-07-27)
///
/// Changed to a record-struct (was previously just a record)
/// This type is not a 1 instance per file scenario.
/// The type is instantiated many times for each file,
/// and then again instantiated each time the file is re-parsed.
/// So the instantiation/lifecycle of this type is lending it to being a struct. (2024-010-02)
/// </summary>
public record struct TextEditorTextSpan(
    int StartingIndexInclusive,
    int EndingIndexExclusive,
    byte DecorationByte,
    ResourceUri ResourceUri,
    string SourceText)
{
    private string? _text;

    /// <summary>
    /// This constructor is used for text spans where their
    /// <see cref="EndingIndexExclusive"/> is the current position
    /// of a <see cref="StringWalker"/>.
    /// </summary>
    public TextEditorTextSpan(
            int StartingIndexInclusive,
            StringWalker stringWalker,
            byte decorationByte)
        : this(
              StartingIndexInclusive,
              stringWalker.PositionIndex,
              decorationByte,
              stringWalker.ResourceUri,
              stringWalker.SourceText)
    {

    }
    
    /// <summary>
    /// This constructor is used for text spans where their
    /// <see cref="EndingIndexExclusive"/> is the current position
    /// of a <see cref="StringWalker"/>.
    /// </summary>
    public TextEditorTextSpan(
            int StartingIndexInclusive,
            ref StringWalkerStruct stringWalker,
            byte decorationByte)
        : this(
              StartingIndexInclusive,
              stringWalker.PositionIndex,
              decorationByte,
              stringWalker.ResourceUri,
              stringWalker.SourceText)
    {

    }
    
    /// <summary>
    /// This constructor is being used to
    /// experiment with not holding a reference
    /// to the <see cref="SourceText"/> (2024-07-27)
    ///
    /// It is a bit clumsy since I'm still taking in
    /// the source text as an argument.
    ///
    /// But the source text and the 'getTextPrecalculatedResult'
    /// share the same datatype and position in the constructor
    /// otherwise.
    /// </summary>
    public TextEditorTextSpan(
            int startingIndexInclusive,
		    int endingIndexExclusive,
		    byte decorationByte,
		    ResourceUri resourceUri,
		    string sourceText,
		    string getTextPrecalculatedResult)
        : this(
              startingIndexInclusive,
              endingIndexExclusive,
              decorationByte,
              resourceUri,
              sourceText)
    {
		_text = getTextPrecalculatedResult;
    }

    public int Length => EndingIndexExclusive - StartingIndexInclusive;

    public string GetText()
    {
        return _text ??= SourceText.Substring(StartingIndexInclusive, Length);
    }
    
    /// <summary>
    /// When using the record 'with' contextual keyword the <see cref="_text"/>
    /// might hold the cached value prior to the 'with' result.
    /// </summary>
    public string ClearTextCache()
    {
        return _text = null;
    }

#if DEBUG
    /// <summary>This expression bound property is useful because it will evaluate <see cref="GetText"/> immediately upon inspecting the object instance in the debugger.</summary>
    [Obsolete("This property is only meant for when running in 'DEBUG' mode and viewing the debugger. One should invoke the method: GetText() instead.")]
    public string Text => GetText();
#endif

    /// <summary>
    /// Preferably one would never use <see cref="FabricateTextSpan"/>.
    /// This method is a hack, and a shortcut.
    /// <br/><br/>
    /// I'm currently (2023-08-03) writing code to 'load' the dotnet runtime assemblies into a CSharpBinder.
    /// This method is useful for now, and worth the technical debt, because
    /// I'm unsure of how to handle the <see cref="TextEditorTextSpan"/> representation
    /// of the metadata I'm reading. There is no file, so what do I do here?
    /// This method will be nice to track all the references to it, so
    /// later I can decide how to handle that.
    /// </summary>
    public static TextEditorTextSpan FabricateTextSpan(string text)
    {
        return new TextEditorTextSpan(
            0,
            text.Length,
            0,
            new("aaa.cs"),
            text);
    }
}