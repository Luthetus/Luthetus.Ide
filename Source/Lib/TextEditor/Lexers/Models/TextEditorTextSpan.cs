namespace Luthetus.TextEditor.RazorLib.Lexers.Models;

/// <summary>
/// If a 'TextEditorTextSpan' is widely shared,
/// that instance might be best served to use 'SetNullSourceText()' on.
/// - It will calculate a new string and cache it which is the substring of this
/// 	text span from the source text.
/// - Then, it will set the reference to the original source text to be 'null' so that the
/// 	memory assocaited with the original source text does not have a reference to it lingering around.
/// It is a question of whether one wants the cost of allocating a new string (the cached substring),
/// or if one would prefer not calculate the substring because it is uncertain whether doing so will be necessary.
/// 
///
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
    int StartInclusiveIndex,
    int EndExclusiveIndex,
    byte DecorationByte,
    ResourceUri ResourceUri,
    string SourceText)
{
    private string? _text;

    /// <summary>
    /// This constructor is used for text spans where their
    /// <see cref="EndExclusiveIndex"/> is the current position
    /// of a <see cref="StringWalker"/>.
    /// </summary>
    public TextEditorTextSpan(
            int StartInclusiveIndex,
            StringWalker stringWalker,
            byte decorationByte)
        : this(
              StartInclusiveIndex,
              stringWalker.PositionIndex,
              decorationByte,
              stringWalker.ResourceUri,
              stringWalker.SourceText)
    {

    }
    
    /// <summary>
    /// This constructor is used for text spans where their
    /// <see cref="EndExclusiveIndex"/> is the current position
    /// of a <see cref="StringWalker"/>.
    /// </summary>
    public TextEditorTextSpan(
            int StartInclusiveIndex,
            ref StringWalkerStruct stringWalker,
            byte decorationByte)
        : this(
              StartInclusiveIndex,
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
            int startInclusiveIndex,
		    int endExclusiveIndex,
		    byte decorationByte,
		    ResourceUri resourceUri,
		    string sourceText,
		    string getTextPrecalculatedResult)
        : this(
              startInclusiveIndex,
              endExclusiveIndex,
              decorationByte,
              resourceUri,
              sourceText)
    {
		_text = getTextPrecalculatedResult;
    }

    public int Length => EndExclusiveIndex - StartInclusiveIndex;
    public bool ConstructorWasInvoked => ResourceUri.Value is not null;

    public string GetText()
    {
        return _text ??= SourceText.Substring(StartInclusiveIndex, Length);
    }
    
    /// <summary>
    /// When using the record 'with' contextual keyword the <see cref="_text"/>
    /// might hold the cached value prior to the 'with' result.
    /// </summary>
    public string ClearTextCache()
    {
        return _text = null;
    }
    
    /// <summary>
    /// The method 'GetText()' will be invoked and cached prior to
    /// setting the 'SourceText' to null.
    ///
    /// This allows one to still get the text from the text span,
    /// but without holding a reference to the original text.
    /// </summary>
    public TextEditorTextSpan SetNullSourceText()
    {
    	_text = GetText();
    	SourceText = null;
    	return this;
    }
    
    /// <summary>
    /// Argument 'text': The pre-calculated text to return when one invokes 'GetText()'
    /// instead of returning a null reference exception.
    /// </summary>
    public TextEditorTextSpan SetNullSourceText(string? text = null)
    {
    	_text = text;
    	SourceText = null;
    	return this;
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