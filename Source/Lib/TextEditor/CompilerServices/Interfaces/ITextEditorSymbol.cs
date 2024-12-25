using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ITextEditorSymbol
{
	/// <summary>
    /// If a symbol references a defintion that exists within a different ResourceUri,
    /// and is not "indexed" information, then this disambiguates the definition.
    ///
    /// Example: TypeClauseNodes reference TypeDefinitionNodes.
    /// If the TypeDefinitionNode being referenced exists in a different file,
    /// this is not an issue because all "top level TypeDefinitionNodes"
    /// are maintained in a global list / dictionary.
    ///
    /// But, if a VariableReferenceNode references a VariableDeclarationNode,
    /// and that 'VariableDeclarationNode' is a property of a 'TypeDefinitionNode',
    /// then this is now too much data to fit onto a symbol (without modifying the symbol type itself).
    ///
    /// So, this property allows one to say, "the 0th symbol I instantiated
    /// is a reference to this TextEditorTextSpan (which is in a different file)."
    ///
    /// The 'int' resets per parse of an entire file. And starts at 0
    /// and increments by 1 each symbol instantiated in that file.
    ///
    /// This requires the 'ISymbol' type to be modified.
    /// But, this solution uses minimal memory since it only requires an 'int', and it is more of an "opt-in".
    /// Since this dictionary property isn't required by any ICompilerService implementations.
    ///
    /// But, if a different ICompilerService implementation wanted to map
    /// from a symbol to arbitrary information, they'd be able to do so.
    /// </summary>
	public int SymbolId { get; }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind { get; }
    /// <summary>
    /// <see cref="SymbolKindString"/> is not an Enum here.
    /// <br/><br/>
    /// This is because <see cref="ITextEditorSymbol"/>
    /// is a class within the Text Editor library;
    /// every enum would have to be specified by the library itself.
    /// <br/><br/>
    /// So, (2023-07-20) this hacky <see cref="string"/> based <see cref="SymbolKindString"/>
    /// is going to be used for now. This allows the consumer of the Text Editor library
    /// to add further SymbolKind(s) of their choosing.
    /// </summary>
    public string SymbolKindString { get; }
}