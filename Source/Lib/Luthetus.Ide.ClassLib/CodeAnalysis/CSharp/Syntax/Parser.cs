using Luthetus.TextEditor.RazorLib.Analysis;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax;

//public class Parser
//{
//    private readonly TokenWalker _tokenWalker;
//    private readonly ImmutableArray<TextEditorDiagnostic> _lexerDiagnostics;
//    private readonly string _sourceText;

//    public Parser(
//            ImmutableArray<ISyntaxToken> tokens,
//            string sourceText,
//            ImmutableArray<TextEditorDiagnostic> lexerDiagnostics)
//    {
//        _sourceText = sourceText;
//        _lexerDiagnostics = lexerDiagnostics;
//        _tokenWalker = new TokenWalker(tokens);
//        _binder = new BinderSession(sourceText);

//        _currentCompilationUnitBuilder = _globalCompilationUnitBuilder;
//    }
//}
