using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
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
    
    public CSharpBinder Binder { get; set; }
    public CSharpBinderSession BinderSession { get; set; }
    public ISyntaxNode RootCodeBlockNode { get; set; }
    
	public List<TextEditorDiagnostic> __DiagnosticList { get; } = new();
    public IReadOnlyList<TextEditorDiagnostic> DiagnosticList => __DiagnosticList;
}
