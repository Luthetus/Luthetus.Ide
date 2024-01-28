using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CompilerServices.States;

public partial record CompilerServiceEditorState
{
    public record SetTextEditorViewModelKeyAction(Key<TextEditorViewModel> TextEditorViewModelKey);
}
