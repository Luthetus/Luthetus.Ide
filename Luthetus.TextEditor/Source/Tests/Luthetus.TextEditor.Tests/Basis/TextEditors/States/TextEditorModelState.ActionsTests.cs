using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;

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

        var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();

        var registerAction = new TextEditorModelState.RegisterAction(
            authenticatedActionKey,
            inModel);

		Assert.Equal(authenticatedActionKey, registerAction.AuthenticatedActionKey);
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
        
        var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();

        var disposeAction = new TextEditorModelState.DisposeAction(
            authenticatedActionKey,
            inModel.ResourceUri);

        Assert.Equal(authenticatedActionKey, disposeAction.AuthenticatedActionKey);
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

        var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();

        var forceRerenderAction = new TextEditorModelState.ForceRerenderAction(
            authenticatedActionKey,
            inModel.ResourceUri);

        Assert.Equal(authenticatedActionKey, forceRerenderAction.AuthenticatedActionKey);
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
        
        var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
        var presentationModel = DiffPresentationFacts.EmptyInPresentationModel;
        
        var registerPresentationModelAction = new TextEditorModelState.RegisterPresentationModelAction(
            authenticatedActionKey,
            inModel.ResourceUri,
            presentationModel);

        Assert.Equal(authenticatedActionKey, registerPresentationModelAction.AuthenticatedActionKey);
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
            var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
			var presentationKey = DiffPresentationFacts.InPresentationKey;

            var calculatePresentationModelAction = new TextEditorModelState.CalculatePresentationModelAction(
                authenticatedActionKey,
                editContext,
				inModel.ResourceUri,
                presentationKey);

            Assert.Equal(authenticatedActionKey, calculatePresentationModelAction.AuthenticatedActionKey);
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
            var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
            var keyboardEventArgs = new KeyboardEventArgs();
            var cancellationToken = CancellationToken.None;

            var keyboardEventAction = new TextEditorModelState.KeyboardEventAction(
                authenticatedActionKey,
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                keyboardEventArgs,
                cancellationToken);

            Assert.Equal(authenticatedActionKey, keyboardEventAction.AuthenticatedActionKey);
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
            var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
            var content = "AlphabetSoup";
            var cancellationToken = CancellationToken.None;

            var insertTextAction = new TextEditorModelState.InsertTextAction(
                authenticatedActionKey,
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                content,
                cancellationToken);

            Assert.Equal(authenticatedActionKey, insertTextAction.AuthenticatedActionKey);
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
            var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
            var motionKind = MotionKind.Backspace;
            var cancellationToken = CancellationToken.None;

            var insertTextAction = new TextEditorModelState.DeleteTextByMotionAction(
                authenticatedActionKey,
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                motionKind,
                cancellationToken);

            Assert.Equal(authenticatedActionKey, insertTextAction.AuthenticatedActionKey);
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
            var authenticatedActionKey = Key<TextEditorAuthenticatedAction>.NewKey();
            var count = 3;
            var cancellationToken = CancellationToken.None;

            var insertTextAction = new TextEditorModelState.DeleteTextByRangeAction(
                authenticatedActionKey,
                editContext,
                inModel.ResourceUri,
                inViewModel.ViewModelKey,
                count,
                cancellationToken);

            Assert.Equal(authenticatedActionKey, insertTextAction.AuthenticatedActionKey);
            Assert.Equal(editContext, insertTextAction.EditContext);
            Assert.Equal(inModel.ResourceUri, insertTextAction.ResourceUri);
            Assert.Equal(inViewModel.ViewModelKey, insertTextAction.ViewModelKey);
            Assert.Equal(count, insertTextAction.Count);
            Assert.Equal(cancellationToken, insertTextAction.CancellationToken);
            return Task.CompletedTask;
        });
	}
}