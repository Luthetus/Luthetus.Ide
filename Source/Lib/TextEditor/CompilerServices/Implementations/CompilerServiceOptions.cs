using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class CompilerServiceOptions
{
    public Func<ResourceUri, ICompilerServiceResource>? RegisterResourceFunc { get; init; }
    /// <summary>
    /// Takes as arguments the resource uri and the source text.
    /// </summary>
    public Func<ICompilerServiceResource, string, ILexer>? GetLexerFunc { get; init; }
    public Func<ICompilerServiceResource, ILexer, IParser>? GetParserFunc { get; init; }
    public Func<ICompilerServiceResource, IParser, IBinder>? GetBinderFunc { get; init; }
    /// <summary>
    /// Invoke an action which is passed the resource which was just lexed.
    /// This invocation occurs within a lock(_resourceMapLock) { ... }
    /// </summary>
    public Action<ICompilerServiceResource, ILexer>? OnAfterLexAction { get; init; }
    /// <summary>
    /// Invoke an action which is passed the resource which was just parsed.
    /// This invocation occurs within a lock(_resourceMapLock) { ... }
    /// </summary>
    public Action<ICompilerServiceResource, CompilationUnit?>? OnAfterParseAction { get; init; }
}
