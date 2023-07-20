using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.CompilerServiceCase;

public class CSharpCompilerService : ICompilerService
{
    private readonly Dictionary<TextEditorModelKey, CSharpResource> _cSharpResourceMap = new();
    private readonly object _cSharpResourceMapLock = new();
    private readonly ICompilerServiceBackgroundTaskQueue _compilerServiceBackgroundTaskQueue;

    public CSharpCompilerService(ICompilerServiceBackgroundTaskQueue compilerServiceBackgroundTaskQueue)
    {
        _compilerServiceBackgroundTaskQueue = compilerServiceBackgroundTaskQueue;
    }

    public void RegisterModel(TextEditorModel model)
    {
        lock (_cSharpResourceMapLock)
        {
            if (_cSharpResourceMap.ContainsKey(model.ModelKey))
                return;

            _cSharpResourceMap.Add(
                model.ModelKey,
                new(model.ModelKey, model.ResourceUri, this));

            QueueParseRequest(model);
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(TextEditorModel model)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(model.ModelKey))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _cSharpResourceMap[model.ModelKey].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(TextEditorModel model)
    {
        lock (_cSharpResourceMapLock)
        {
            if (!_cSharpResourceMap.ContainsKey(model.ModelKey))
                return ImmutableArray<ITextEditorSymbol>.Empty;

            return _cSharpResourceMap[model.ModelKey].Symbols;
        }
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnosticsFor(TextEditorModel model)
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public void ModelWasModified(TextEditorModel model, ImmutableArray<TextEditorTextSpan> editTextSpans)
    {
        throw new NotImplementedException();
    }

    public void DisposeModel(TextEditorModel model)
    {
        lock (_cSharpResourceMapLock)
        {
            _cSharpResourceMap.Remove(model.ModelKey);
        }
    }

    private void QueueParseRequest(TextEditorModel model)
    {
        var parseBackgroundWorkItem = new BackgroundTask(
            async cancellationToken =>
            {
                var lexer = new CSharpLexer(model.ResourceUri, model.GetAllText());
                lexer.Lex();

                var parser = new CSharpParser(lexer);
                var compilationUnit = parser.Parse();

                if (compilationUnit is null)
                    return;

                lock (_cSharpResourceMapLock)
                {
                    if (!_cSharpResourceMap.ContainsKey(model.ModelKey))
                        return;

                    var cSharpResource = _cSharpResourceMap[model.ModelKey];

                    cSharpResource.CompilationUnit = compilationUnit;
                    cSharpResource.SyntaxTokens = lexer.SyntaxTokens;
                }

                await model.ApplySyntaxHighlightingAsync();

                return;
            },
            "TODO: name",
            "TODO: description",
            false,
            _ => Task.CompletedTask,
            null,
            CancellationToken.None);

        _compilerServiceBackgroundTaskQueue.QueueBackgroundWorkItem(parseBackgroundWorkItem);
    }
}
