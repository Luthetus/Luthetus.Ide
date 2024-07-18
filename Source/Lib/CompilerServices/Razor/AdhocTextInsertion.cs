using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.Razor;

public class AdhocTextInsertion
{
    public AdhocTextInsertion(
        string content,
        int sourceTextStartingIndexInclusive,
        int insertionStartingIndexInclusive,
        StringWalker stringWalker)
    {
        Content = content;
        SourceTextStartingIndexInclusive = sourceTextStartingIndexInclusive;
        InsertionStartingIndexInclusive = insertionStartingIndexInclusive;
        StringWalker = stringWalker;
    }

    public string Content { get; }
    public int SourceTextStartingIndexInclusive { get; }
    public int InsertionStartingIndexInclusive { get; set; }
    public StringWalker StringWalker { get; }

    public int InsertionEndingIndexExclusive => InsertionStartingIndexInclusive + Content.Length;

    public static AdhocTextInsertion PerformInsertion(
        string content,
        int sourceTextStartingIndexInclusive,
        StringBuilder stringBuilder,
        StringWalker stringWalker)
    {
        var adhocTextInsertion = new AdhocTextInsertion(
                content,
                sourceTextStartingIndexInclusive,
                stringBuilder.Length,
                stringWalker);

        stringBuilder.Append(content);

        return adhocTextInsertion;
    }
}