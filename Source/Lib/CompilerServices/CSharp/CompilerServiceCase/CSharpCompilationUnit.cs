using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.CSharp.LexerCase;
using Luthetus.CompilerServices.CSharp.ParserCase;
using Luthetus.CompilerServices.CSharp.BinderCase;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public sealed class CSharpCompilationUnit : ICompilationUnit
{
	public CSharpCompilationUnit(
		ResourceUri resourceUri,
		CSharpBinder binder)
	{
		ResourceUri = resourceUri;
		Binder = binder;
	}

	public ResourceUri ResourceUri { get; set; }
    public CSharpLexerOutput LexerOutput { get; set; }
    public CSharpParser Parser { get; set; }
    public CSharpParserModel ParserModel { get; set; }
    public CSharpBinder Binder { get; set; }
    public CSharpBinderSession BinderSession { get; set; }
    public CodeBlockNode RootCodeBlockNode { get; set; }
    public ImmutableArray<TextEditorDiagnostic> DiagnosticsList { get; init; }
}