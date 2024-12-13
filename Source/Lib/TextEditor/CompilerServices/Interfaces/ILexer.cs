using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

/// <summary>
/// Idea...
/// =======
/// 
/// Preface of the idea:
/// ====================
/// The CSharpLexer is constructed within a function invocation,
/// and is never exposed beyond the scope of that function invocation.
///
/// So, perhaps instead of getting the new CSharpLexer() invocations
/// to be 1 time instead of 1,814 times.
///
/// It might be fine to make it a struct.
///
/// Issue with the idea:
/// ====================
/// I don't want to force every implementation of ILexer to be a struct.
/// But, at the same time if I reference an ILexer implementation
/// not by its concrete-struct type, but by the interface,
/// then that struct will be boxed anyway.
///
/// How to continue with the idea:
/// ==============================
/// The ICompilerService has the method:
/// ````public Task ParseAsync(ITextEditorEditContext editContext, TextEditorModelModifier modelModifier);
///
/// Furthermore there is a 'default' implementation of ICompilerService that is available for use by anyone.
/// It is named 'CompilerService'.
///
/// It implements 'ParseAsync(...)' but adds the 'virtual' keyword so that any inheriting type can override it.
/// 
/// Currently, the CSharpCompilerService inherits 'CompilerService' (not the interface).
///
/// In order to permit any language to "easily" create an implementation of 'ICompilerService'
/// they can inherit 'CompilerService' and then override only what they want to be different
/// from the default setup.
///
/// The result of this however is fairly "general" Func<...> that need to be defined
/// in order to tell the 'CompilerService' how to construct an instance of the language's
/// ILexer, IParser, IBinder...
///
/// So, if one wants to streamline the implementation of 'ICompilerService'
/// by inheriting 'CompilerService' instead,
/// then would they be able to avoid the ILexer, IParser, IBinder
/// requirements of the 'CompilerService' base class?
/// (Do this in order to avoid boxing of the struct implementation of ILexer or etc...)
///
/// The 'CompilerService' only invokes the funcs that provide an instance of ILexer and etc...
/// from within 'ParseAsync(...)'.
/// So, one can completely forgo the interface Type and do everything themselves
/// by overriding 'ParseAsync(...)' in particular.
/// ("everything" refers to the parsing itself, a lot of defaults are still
/// being provided by the 'CompilerService' base class).
/// 
/// Side note 1:
/// ============
/// What benefits are there to having the parser ask the lexer for the next token
/// during the lexing process, versus lexing the text entirely and
/// storing the tokens in a list to then later be given to the parser?
///
/// Side note 2:
/// ============
/// The CSharpLexer is inheriting 'Lexer' at the moment.
/// But, this was only done in order provide defaults for someone
/// who wants to create an ILexer implementation.
///
/// The best case scenario is to implement the interface ILexer on CSharpLexer
/// directly.
///
/// Furthermore, if one overrides 'ParseAsync(...)' they don't even
/// have to implement ILexer at all. They have complete freedom over the 'ParseAsync(...)' method.
/// </summary>
public interface ILexer
{
    public List<TextEditorDiagnostic> DiagnosticList { get; }
    public List<ISyntaxToken> SyntaxTokenList { get; }
    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LexerKeywords LexerKeywords { get; }

    public void Lex();
}
