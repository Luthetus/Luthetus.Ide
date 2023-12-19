using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class SomeClass
{
    private readonly TextEditorService _textEditorService;

    public SomeClass(TextEditorService textEditorService)
    {
        _textEditorService = textEditorService;
    }

    public void Aaa()
    {
        var edit = _textEditorService.CreateEdit(async edit =>
        {
            /* Scope the state here */

            var redundantEdit = RedundantEdit();

            await edit.Execute(redundantEdit);
            await edit.Execute(redundantEdit);
            ;
        });

        _textEditorService.EnqueueEdit(edit);

        var aaaEdit = _textEditorService.CreateEdit(async edit =>
        {
            /* Scope the state here */
            (await edit.Model_SetResourceData(resour))
                .UndoEdit()
                .Reload();
        });

        _textEditorService.EnqueueEdit(RedundantEdit());
    }

    public ITextEditorEdit RedundantEdit()
    {
        var resourceUri = new ResourceUri("/abc.txt");

        return _textEditorService.CreateEdit(async edit =>
        {
            /* Scope the state here */
            await edit.Model_UndoEdit(resourceUri);
            await edit.Model_RedoEdit(resourceUri);
        });
    }
}

