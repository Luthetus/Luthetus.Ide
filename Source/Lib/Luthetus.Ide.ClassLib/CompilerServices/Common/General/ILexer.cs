using Luthetus.TextEditor.RazorLib.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public interface ILexer
{
    public ImmutableArray<TextEditorDiagnostic> Diagnostics { get; }
}
