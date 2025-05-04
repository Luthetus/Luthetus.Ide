using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Razor;

public class AdhocTextInsertion
{
    public AdhocTextInsertion(
        string content,
        int sourceText_StartInclusiveIndex,
        int insertion_StartInclusiveIndex,
        StringWalker stringWalker)
    {
        Content = content;
        SourceText_StartInclusiveIndex = sourceText_StartInclusiveIndex;
        Insertion_StartInclusiveIndex = insertion_StartInclusiveIndex;
        StringWalker = stringWalker;
    }

    public string Content { get; }
    public int SourceText_StartInclusiveIndex { get; }
    public int Insertion_StartInclusiveIndex { get; set; }
    public StringWalker StringWalker { get; }

    public int Insertion_EndExclusiveIndex => Insertion_StartInclusiveIndex + Content.Length;

    public static AdhocTextInsertion PerformInsertion(
        string content,
        int sourceText_StartInclusiveIndex,
        StringBuilder stringBuilder,
        StringWalker stringWalker)
    {
        var adhocTextInsertion = new AdhocTextInsertion(
			content,
			sourceText_StartInclusiveIndex,
			stringBuilder.Length,
			stringWalker);

        stringBuilder.Append(content);

        return adhocTextInsertion;
    }
}