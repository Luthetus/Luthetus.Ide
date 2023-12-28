using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

/// <summary>
/// <see cref="TextEditorModel"/>
/// </summary>
public class TextEditorModelPublicMethodsTests
{
	/// <summary>
	/// <see cref="TextEditorModel.PerformForceRerenderAction(TextEditorModelState.ForceRerenderAction)"/>
	/// </summary>
	[Fact]
	public void PerformForceRerenderAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var modelModifier = new TextEditorModelModifier(inModel);

		var outModel = modelModifier.ForceRerenderAction();

		Assert.NotEqual(inModel, outModel);
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformEditTextEditorAction(TextEditorModelState.KeyboardEventAction, RazorLib.TextEditors.Models.TextEditorCursorModifierBag)"/>
	/// </summary>
	[Fact]
	public void PerformEditTextEditorAction_KeyboardEventAction()
	{
  //      TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
  //          out var textEditorService,
  //          out var inModel,
  //          out var inViewModel,
  //          out var serviceProvider);

		//textEditorService.Post(editContext =>
		//{
		//	var modelModifier = editContext.GetModelModifier(inModel.ResourceUri);

		//	if (modelModifier is null)
		//	{

		//	}

  //          inModel.PerformEditTextEditorAction(new TextEditorModelState.KeyboardEventAction(
  //              ));
  //      });

		

        throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformEditTextEditorAction(TextEditorModelState.InsertTextAction, TextEditorCursorModifierBag)"/>
	/// </summary>
	[Fact]
	public void PerformEditTextEditorAction_InsertTextAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformEditTextEditorAction(TextEditorModelState.DeleteTextByMotionAction, TextEditorCursorModifierBag)"/>
	/// </summary>
	[Fact]
	public void PerformEditTextEditorAction_DeleteTextByMotionAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformEditTextEditorAction(TextEditorModelState.DeleteTextByRangeAction, TextEditorCursorModifierBag)"/>
	/// </summary>
	[Fact]
	public void PerformEditTextEditorAction_DeleteTextByRangeAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformRegisterPresentationModelAction(TextEditorModelState.RegisterPresentationModelAction)"/>
	/// </summary>
	[Fact]
	public void PerformRegisterPresentationModelAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.PerformCalculatePresentationModelAction(TextEditorModelState.CalculatePresentationModelAction)"/>
	/// </summary>
	[Fact]
	public void PerformCalculatePresentationModelAction()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.SetDecorationMapper(IDecorationMapper)"/>
	/// </summary>
	[Fact]
	public void SetDecorationMapper()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.SetCompilerService(ICompilerService)"/>
	/// </summary>
	[Fact]
	public void SetCompilerService()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.SetTextEditorSaveFileHelper(TextEditorSaveFileHelper)"/>
	/// </summary>
	[Fact]
	public void SetTextEditorSaveFileHelper()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.SetResourceData(ResourceUri, DateTime)"/>
	/// </summary>
	[Fact]
	public void SetResourceData()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.SetUsingRowEndingKind(RowEndingKind)"/>
	/// </summary>
	[Fact]
	public void SetUsingRowEndingKind()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.ClearEditBlocks()"/>
	/// </summary>
	[Fact]
	public void ClearEditBlocks()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.UndoEdit()"/>
	/// </summary>
	[Fact]
	public void UndoEdit()
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// <see cref="TextEditorModel.RedoEdit()"/>
	/// </summary>
	[Fact]
	public void RedoEdit()
	{
		throw new NotImplementedException();
	}
}