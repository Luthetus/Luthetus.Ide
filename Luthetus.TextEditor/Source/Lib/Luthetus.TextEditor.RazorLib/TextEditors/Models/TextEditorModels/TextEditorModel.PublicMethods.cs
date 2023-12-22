using Luthetus.TextEditor.RazorLib.Rows.Models;
using static Luthetus.TextEditor.RazorLib.TextEditors.States.TextEditorModelState;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel
{
    public TextEditorModel PerformForceRerenderAction(ForceRerenderAction forceRerenderAction)
    {
        var modelModifier = new TextEditorModelModifier(this);
        return modelModifier.ToModel();
    }

    public TextEditorModel PerformEditTextEditorAction(
        KeyboardEventAction keyboardEventAction,
        TextEditorCursorModifierBag cursorModifierBag)
    {
        var modelModifier = new TextEditorModelModifier(this);
        modelModifier.PerformEditTextEditorAction(keyboardEventAction, cursorModifierBag);
		return modelModifier.ToModel();
    }

    public TextEditorModel PerformEditTextEditorAction(InsertTextAction insertTextAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformEditTextEditorAction(insertTextAction);
		return modelModifier.ToModel();
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByMotionAction deleteTextByMotionAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformEditTextEditorAction(deleteTextByMotionAction);
		return modelModifier.ToModel();
    }

    public TextEditorModel PerformEditTextEditorAction(DeleteTextByRangeAction deleteTextByRangeAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
        modelModifier.PerformEditTextEditorAction(deleteTextByRangeAction);
		return modelModifier.ToModel();
    }

    public TextEditorModel PerformRegisterPresentationModelAction(RegisterPresentationModelAction registerPresentationModelAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformRegisterPresentationModelAction(registerPresentationModelAction);
		return modelModifier.ToModel();
    }
    
    public TextEditorModel PerformCalculatePresentationModelAction(CalculatePresentationModelAction calculatePresentationModelAction)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.PerformCalculatePresentationModelAction(calculatePresentationModelAction);
		return modelModifier.ToModel();
    }

    public TextEditorModel SetDecorationMapper(IDecorationMapper decorationMapper)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyDecorationMapper(decorationMapper);
		return modelModifier.ToModel();
    }

    public TextEditorModel SetCompilerService(ICompilerService compilerService)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyCompilerService(compilerService);
		return modelModifier.ToModel();
    }
    
    public TextEditorModel SetTextEditorSaveFileHelper(TextEditorSaveFileHelper textEditorSaveFileHelper)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyTextEditorSaveFileHelper(textEditorSaveFileHelper);
		return modelModifier.ToModel();
    }

    public TextEditorModel SetResourceData(ResourceUri resourceUri, DateTime resourceLastWriteTime)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyResourceData(resourceUri, resourceLastWriteTime);
		return modelModifier.ToModel();
    }

    public TextEditorModel SetUsingRowEndingKind(RowEndingKind rowEndingKind)
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ModifyUsingRowEndingKind(rowEndingKind);
		return modelModifier.ToModel();
    }

    public TextEditorModel ClearEditBlocks()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.ClearEditBlocks();
		return modelModifier.ToModel();
    }

    /// <summary>The "if (EditBlockIndex == _editBlocksPersisted.Count)"<br/><br/>Is done because the active EditBlock is not yet persisted.<br/><br/>The active EditBlock is instead being 'created' as the user continues to make edits of the same <see cref="TextEditKind"/><br/><br/>For complete clarity, this comment refers to one possibly expecting to see "if (EditBlockIndex == _editBlocksPersisted.Count - 1)"</summary>
    public TextEditorModel UndoEdit()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.UndoEdit();
		return modelModifier.ToModel();
    }

    public TextEditorModel RedoEdit()
    {
		var modelModifier = new TextEditorModelModifier(this);
		modelModifier.RedoEdit();
		return modelModifier.ToModel();
    }
}