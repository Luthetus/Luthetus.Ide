using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class PreprocessorLibraryReferenceStatementNode : ISyntaxNode
{
	public PreprocessorLibraryReferenceStatementNode(
		SyntaxToken includeDirectiveSyntaxToken,
		SyntaxToken libraryReferenceSyntaxToken)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.PreprocessorLibraryReferenceStatementNode++;
		#endif
	
		IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
		LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;
	}

	public SyntaxToken IncludeDirectiveSyntaxToken { get; }
	public SyntaxToken LibraryReferenceSyntaxToken { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
}