using Xunit;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.States;

/// <summary>
/// <see cref="TextEditorViewModelState"/>
/// </summary>
public class TextEditorViewModelStateActionsTests
{
    /// <summary>
    /// <see cref="TextEditorViewModelState.RegisterAction"/>
    /// </summary>
    [Fact]
    public void RegisterAction()
    {
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var registerAction = new TextEditorViewModelState.RegisterAction(
            inViewModel.ViewModelKey,
            inModel.ResourceUri,
            textEditorService);

        Assert.Equal(inViewModel.ViewModelKey, registerAction.ViewModelKey);
        Assert.Equal(inModel.ResourceUri, registerAction.ResourceUri);
        Assert.Equal(textEditorService, registerAction.TextEditorService);
    }

    /// <summary>
    /// <see cref="TextEditorViewModelState.DisposeAction"/>
    /// </summary>
    [Fact]
	public void DisposeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var disposeAction = new TextEditorViewModelState.DisposeAction(
            inViewModel.ViewModelKey);

        Assert.Equal(inViewModel.ViewModelKey, disposeAction.ViewModelKey);
	}

	/// <summary>
	/// <see cref="TextEditorViewModelState.SetViewModelWithAction"/>
	/// </summary>
	[Fact]
	public void SetViewModelWithAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        textEditorService.Post(nameof(SetViewModelWithAction), editContext =>
        {
            var authenticatedActionKey = TextEditorService.AuthenticatedActionKey;
            var withFunc = new Func<TextEditorViewModel, TextEditorViewModel>(inState => inState);
            
            var setViewModelWithAction = new TextEditorViewModelState.SetViewModelWithAction(
                authenticatedActionKey,
                editContext,
                inViewModel.ViewModelKey,
                withFunc);

            Assert.Equal(editContext, setViewModelWithAction.EditContext);
            Assert.Equal(inViewModel.ViewModelKey, setViewModelWithAction.ViewModelKey);
            Assert.Equal(withFunc, setViewModelWithAction.WithFunc);

            return Task.CompletedTask;
        });
	}
}