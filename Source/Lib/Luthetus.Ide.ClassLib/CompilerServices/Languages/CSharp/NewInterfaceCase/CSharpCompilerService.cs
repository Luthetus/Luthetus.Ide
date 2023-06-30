using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.TextEditor.RazorLib.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.NewInterfaceCase;

public class CSharpCompilerService : ICompilerService
{
    private readonly Dictionary<TextEditorModelKey, CSharpResource> _cSharpResourceMap = new();
    private readonly object _cSharpResourceMapLock = new();

    public void RegisterModel(TextEditorModel textEditorModel)
    {
        lock (_cSharpResourceMapLock)
        {
            if (_cSharpResourceMap.ContainsKey(textEditorModel.ModelKey))
                return;

            _cSharpResourceMap.Add(
                textEditorModel.ModelKey,
                new(textEditorModel.ModelKey, textEditorModel.ResourceUri, this));
        }
    }

    public ImmutableArray<TextEditorTextSpan> GetDiagnosticTextSpansFor(TextEditorModel textEditorModel)
    {
        throw new NotImplementedException();
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbolsFor(TextEditorModel textEditorModel)
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
}
