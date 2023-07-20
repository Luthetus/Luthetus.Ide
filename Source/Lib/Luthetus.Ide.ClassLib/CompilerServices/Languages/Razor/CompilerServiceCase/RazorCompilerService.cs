using Luthetus.Common.RazorLib.BackgroundTaskCase.BaseTypes;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.HostedServiceCase.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.Razor.CompilerServiceCase;

public class RazorCompilerService : ICompilerService
{
    private readonly Dictionary<TextEditorModelKey, RazorResource> _razorResourceMap = new();
    private readonly object _razorResourceMapLock = new();
    private readonly ICompilerServiceBackgroundTaskQueue _compilerServiceBackgroundTaskQueue;
    private readonly CSharpCompilerService _cSharpCompilerService;

    public RazorCompilerService(
        ICompilerServiceBackgroundTaskQueue compilerServiceBackgroundTaskQueue,
        CSharpCompilerService cSharpCompilerService)
    {
        _compilerServiceBackgroundTaskQueue = compilerServiceBackgroundTaskQueue;
        _cSharpCompilerService = cSharpCompilerService;
    }

    public void RegisterModel(TextEditorModel model)
    {
        lock (_razorResourceMapLock)
        {
            if (_razorResourceMap.ContainsKey(model.ModelKey))
                return;

            _razorResourceMap.Add(
                model.ModelKey,
                new(model.ModelKey, model.ResourceUri, this));

            QueueParseRequest(model);
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpansFor(TextEditorModel model)
    {
        lock (_razorResourceMapLock)
        {
            if (!_razorResourceMap.ContainsKey(model.ModelKey))
                return ImmutableArray<TextEditorTextSpan>.Empty;

            return _razorResourceMap[model.ModelKey].SyntacticTextSpans;
        }
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(TextEditorModel model)
    {
        return ImmutableArray<ITextEditorSymbol>.Empty;
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
        lock (_razorResourceMapLock)
        {
            _razorResourceMap.Remove(model.ModelKey);
        }
    }

    private void QueueParseRequest(TextEditorModel model)
    {
        var parseBackgroundWorkItem = new BackgroundTask(
            async cancellationToken =>
            {
                var lexer = new RazorLexer(model.ResourceUri);
                var syntacticTextSpans = await lexer.Lex(model.GetAllText(), model.RenderStateKey);
                
                lock (_razorResourceMapLock)
                {
                    if (!_razorResourceMap.ContainsKey(model.ModelKey))
                        return;

                    var razorResource = _razorResourceMap[model.ModelKey];

                    razorResource.SyntacticTextSpans = syntacticTextSpans;
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
