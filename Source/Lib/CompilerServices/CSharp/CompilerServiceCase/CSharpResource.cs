using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public class CSharpResource : CompilerServiceResource
{
    public CSharpResource(ResourceUri resourceUri, CSharpCompilerService cSharpCompilerService)
        : base(resourceUri, cSharpCompilerService)
    {
    }

	public IReadOnlyList<TextEditorTextSpan> EscapeCharacterList { get; internal set; }

	public override IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
		var tokenTextSpanList = new List<TextEditorTextSpan>();

        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));
		tokenTextSpanList.AddRange(EscapeCharacterList);

		return tokenTextSpanList;
    }
}