using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthCompilerServiceOptions
{
    public Func<ResourceUri, ILuthCompilerServiceResource>? RegisterResourceFunc { get; init; }
    /// <summary>
    /// Takes as arguments the resource uri and the source text.
    /// </summary>
    public Func<ILuthCompilerServiceResource, string, ILuthLexer>? GetLexerFunc { get; init; }
    public Func<ILuthCompilerServiceResource, ILuthLexer, ILuthParser>? GetParserFunc { get; init; }
    public Func<ILuthCompilerServiceResource, ILuthParser, ILuthBinder>? GetBinderFunc { get; init; }
    /// <summary>
    /// Invoke an action which is passed the resource which was just lexed.
    /// This invocation occurs within a lock(_resourceMapLock) { ... }
    /// </summary>
    public Action<ILuthCompilerServiceResource, ILuthLexer>? OnAfterLexAction { get; init; }
    /// <summary>
    /// Invoke an action which is passed the resource which was just parsed.
    /// This invocation occurs within a lock(_resourceMapLock) { ... }
    /// </summary>
    public Action<ILuthCompilerServiceResource, CompilationUnit?>? OnAfterParseAction { get; init; }
}
