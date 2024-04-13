using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.TextEditor.RazorLib.Lexes.Models;

public record TextEditorTextSpan(
    int StartingIndexInclusive,
    int EndingIndexExclusive,
    byte DecorationByte,
    ResourceUri ResourceUri,
    string SourceText)
{
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

#if DEBUG
    /// <summary>This expression bound property is useful because it will evaluate <see cref="GetText"/> immediately upon inspecting the object instance in the debugger.</summary>
    [Obsolete("This property is only meant for when running in 'DEBUG' mode and viewing the debugger. One should invoke the method: GetText() instead.")]
    public string Text => GetText();
#endif

    public int Length => EndingIndexExclusive - StartingIndexInclusive;

    public string GetText()
    {
        return SourceText.Substring(
            StartingIndexInclusive,
            Length);
    }

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