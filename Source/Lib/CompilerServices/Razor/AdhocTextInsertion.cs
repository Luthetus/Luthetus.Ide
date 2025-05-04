using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.Razor;

public class AdhocTextInsertion
{
    public AdhocTextInsertion(
        string content,
        int sourceTextStartInclusiveIndex,
        int insertionStartInclusiveIndex,
        StringWalker stringWalker)
    {
        Content = content;
        SourceTextStartInclusiveIndex = sourceTextStartInclusiveIndex;
        InsertionStartInclusiveIndex = insertionStartInclusiveIndex;
        StringWalker = stringWalker;
    }

    public string Content { get; }
    public int SourceTextStartInclusiveIndex { get; }
    public int InsertionStartInclusiveIndex { get; set; }
    public StringWalker StringWalker { get; }

    public int InsertionEndExclusiveIndex => InsertionStartInclusiveIndex + Content.Length;

    public static AdhocTextInsertion PerformInsertion(
        string content,
        int sourceTextStartInclusiveIndex,
        StringBuilder stringBuilder,
        StringWalker stringWalker)
    {
        var adhocTextInsertion = new AdhocTextInsertion(
			content,
			sourceTextStartInclusiveIndex,
			stringBuilder.Length,
			stringWalker);

        stringBuilder.Append(content);

        return adhocTextInsertion;
    }
}