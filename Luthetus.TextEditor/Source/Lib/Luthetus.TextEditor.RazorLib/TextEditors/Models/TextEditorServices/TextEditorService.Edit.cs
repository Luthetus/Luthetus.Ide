using Fluxor;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using static Luthetus.TextEditor.RazorLib.Commands.Models.TextEditorCommand;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using System.Reflection;
using static Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorService;
using Microsoft.JSInterop;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorService
{
    private class TextEditorEdit : ITextEditorEdit
    {
        private readonly TextEditorService _textEditorService;
        
        public TextEditorEdit(TextEditorService textEditorService, Func<ITextEditorEditContext, Task> func)
        {
            _textEditorService = textEditorService;
            Func = func;
        }

        public readonly Func<ITextEditorEditContext, Task> Func;

        public async Task ExecuteAsync(ITextEditorEditContext editContext)
        {
            await Func.Invoke(editContext);
        }
    }
}

