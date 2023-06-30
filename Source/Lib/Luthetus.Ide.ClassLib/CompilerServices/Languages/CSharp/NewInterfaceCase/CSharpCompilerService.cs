using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.ClassLib.CompilerServices.HostedServiceCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.ParserCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.NewInterfaceCase;

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

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(TextEditorModel textEditorModel)
    {
        throw new NotImplementedException();
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(TextEditorModel textEditorModel)
    {
        throw new NotImplementedException();
    }

    public ImmutableArray<TextEditorTextSpan> GetDiagnosticTextSpansFor(TextEditorModel textEditorModel)
    {
        throw new NotImplementedException();
    }

    public void ModelWasModified(TextEditorModel textEditorModel, ImmutableArray<TextEditorTextSpan> editTextSpans)
    {
        throw new NotImplementedException();
    }

    public void DisposeModel(TextEditorModel textEditorModel)
    {
        lock (_cSharpResourceMapLock)
        {
            _cSharpResourceMap.Remove(textEditorModel.ModelKey);
        }
    }

    private void QueueParseRequest(TextEditorModel model)
    {
        var parseBackgroundWorkItem = new BackgroundTask(
            async cancellationToken =>
            {
                var lexer = new CSharpLexer(model.ResourceUri, model.GetAllText());
                lexer.Lex();

                var parser = new CSharpParser(lexer.SyntaxTokens, lexer.Diagnostics);
                var compilationUnit = parser.Parse();

                if (compilationUnit is null)
                    return;

                lock (_cSharpResourceMapLock)
                {
                    if (!_cSharpResourceMap.ContainsKey(model.ModelKey))
                        return;

                    _cSharpResourceMap[model.ModelKey]
                        .CompilationUnit = compilationUnit;
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
