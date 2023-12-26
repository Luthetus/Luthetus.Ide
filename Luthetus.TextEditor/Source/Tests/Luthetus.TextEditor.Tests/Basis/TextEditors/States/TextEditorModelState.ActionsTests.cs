using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorModelState"/>
/// </summary>
public class TextEditorModelStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorModelState.RegisterAction"/>
	/// </summary>
	[Fact]
	public void RegisterAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var registerAction = new TextEditorModelState.RegisterAction(inModel);
		Assert.Equal(inModel, registerAction.Model);
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DisposeAction"/>
	/// </summary>
	[Fact]
	public void DisposeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var disposeAction = new TextEditorModelState.DisposeAction(inModel.ResourceUri);
        Assert.Equal(inModel.ResourceUri, disposeAction.ResourceUri);
	}

	/// <summary>
	/// <see cref="TextEditorModelState.ForceRerenderAction"/>
	/// </summary>
	[Fact]
	public void ForceRerenderAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var forceRerenderAction = new TextEditorModelState.ForceRerenderAction(
            inModel.ResourceUri);

        Assert.Equal(inModel.ResourceUri, forceRerenderAction.ResourceUri);
    }

	/// <summary>
	/// <see cref="TextEditorModelState.RegisterPresentationModelAction"/>
	/// </summary>
	[Fact]
	public void RegisterPresentationModelAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var presentationModel = DiffPresentationFacts.EmptyInPresentationModel;
        var registerPresentationModelAction = new TextEditorModelState.RegisterPresentationModelAction(
            inModel.ResourceUri,
            presentationModel);

        Assert.Equal(inModel.ResourceUri, registerPresentationModelAction.ResourceUri);
        Assert.Equal(presentationModel, registerPresentationModelAction.PresentationModel);
    }

	/// <summary>
	/// <see cref="TextEditorModelState.CalculatePresentationModelAction"/>
	/// </summary>
	[Fact]
	public void CalculatePresentationModelAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(editContext =>
        {
			var presentationKey = DiffPresentationFacts.InPresentationKey;
            var calculatePresentationModelAction = new TextEditorModelState.CalculatePresentationModelAction(
                editContext,
				inModel.ResourceUri,
                presentationKey);

            Assert.Equal(editContext, calculatePresentationModelAction.EditContext);
            Assert.Equal(inModel.ResourceUri, calculatePresentationModelAction.ResourceUri);
            Assert.Equal(presentationKey, calculatePresentationModelAction.PresentationKey);
            return Task.CompletedTask;
        });
	}

	/// <summary>
	/// <see cref="TextEditorModelState.KeyboardEventAction"/>
	/// </summary>
	[Fact]
	public void KeyboardEventAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(editContext =>
        {
            var keyboardEventArgs = new KeyboardEventArgs();
            var cancellationToken = CancellationToken.None;
            var keyboardEventAction = new TextEditorModelState.KeyboardEventAction(
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                keyboardEventArgs,
                cancellationToken);

            Assert.Equal(editContext, keyboardEventAction.EditContext);
            Assert.Equal(inModel.ResourceUri, keyboardEventAction.ResourceUri);
            Assert.Equal(inViewModel.ViewModelKey, keyboardEventAction.ViewModelKey);
            Assert.Equal(keyboardEventArgs, keyboardEventAction.KeyboardEventArgs);
            Assert.Equal(cancellationToken, keyboardEventAction.CancellationToken);
            return Task.CompletedTask;
        });
	}

	/// <summary>
	/// <see cref="TextEditorModelState.InsertTextAction"/>
	/// </summary>
	[Fact]
	public void InsertTextAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(editContext =>
        {
            var content = "AlphabetSoup";
            var cancellationToken = CancellationToken.None;
            var insertTextAction = new TextEditorModelState.InsertTextAction(
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                content,
                cancellationToken);

            Assert.Equal(editContext, insertTextAction.EditContext);
            Assert.Equal(inModel.ResourceUri, insertTextAction.ResourceUri);
            Assert.Equal(inViewModel.ViewModelKey, insertTextAction.ViewModelKey);
            Assert.Equal(content, insertTextAction.Content);
            Assert.Equal(cancellationToken, insertTextAction.CancellationToken);
            return Task.CompletedTask;
        });
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DeleteTextByMotionAction"/>
	/// </summary>
	[Fact]
	public void DeleteTextByMotionAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(editContext =>
        {
            var motionKind = MotionKind.Backspace;
            var cancellationToken = CancellationToken.None;
            var insertTextAction = new TextEditorModelState.DeleteTextByMotionAction(
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                motionKind,
                cancellationToken);

            Assert.Equal(editContext, insertTextAction.EditContext);
            Assert.Equal(inModel.ResourceUri, insertTextAction.ResourceUri);
            Assert.Equal(inViewModel.ViewModelKey, insertTextAction.ViewModelKey);
            Assert.Equal(motionKind, insertTextAction.MotionKind);
            Assert.Equal(cancellationToken, insertTextAction.CancellationToken);
            return Task.CompletedTask;
        });
	}

	/// <summary>
	/// <see cref="TextEditorModelState.DeleteTextByRangeAction"/>
	/// </summary>
	[Fact]
	public void DeleteTextByRangeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(editContext =>
        {
            var count = 3;
            var cancellationToken = CancellationToken.None;
            var insertTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                count,
                cancellationToken);

            Assert.Equal(editContext, insertTextAction.EditContext);
            Assert.Equal(inModel.ResourceUri, insertTextAction.ResourceUri);
            Assert.Equal(inViewModel.ViewModelKey, insertTextAction.ViewModelKey);
            Assert.Equal(count, insertTextAction.Count);
            Assert.Equal(cancellationToken, insertTextAction.CancellationToken);
            return Task.CompletedTask;
        });
	}
}