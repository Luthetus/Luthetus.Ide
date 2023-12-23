using Xunit;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.Tests.Basis.Groups.States;

/// <summary>
/// <see cref="TextEditorGroupState"/>
/// </summary>
public class TextEditorGroupStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorGroupState.RegisterAction"/>
	/// </summary>
	[Fact]
	public void RegisterAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var group = new TextEditorGroup(
			Key<TextEditorGroup>.NewKey(),
            inViewModel.ViewModelKey,
			new Key<TextEditorViewModel>[] { inViewModel.ViewModelKey }.ToImmutableList());

		var registerAction = new TextEditorGroupState.RegisterAction(group);

		Assert.Equal(group, registerAction.Group);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.DisposeAction"/>
	/// </summary>
	[Fact]
	public void DisposeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

		var groupKey = Key<TextEditorGroup>.NewKey();

        var group = new TextEditorGroup(
            groupKey,
            inViewModel.ViewModelKey,
            new Key<TextEditorViewModel>[] { inViewModel.ViewModelKey }.ToImmutableList());

        var disposeAction = new TextEditorGroupState.DisposeAction(group.GroupKey);

        Assert.Equal(groupKey, disposeAction.GroupKey);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.AddViewModelToGroupAction"/>
	/// </summary>
	[Fact]
	public void AddViewModelToGroupAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var groupKey = Key<TextEditorGroup>.NewKey();

        var group = new TextEditorGroup(
            groupKey,
            inViewModel.ViewModelKey,
            new Key<TextEditorViewModel>[] { inViewModel.ViewModelKey }.ToImmutableList());

        var addViewModelToGroupAction = new TextEditorGroupState.AddViewModelToGroupAction(
			group.GroupKey,
			inViewModel.ViewModelKey);

        Assert.Equal(groupKey, addViewModelToGroupAction.GroupKey);
        Assert.Equal(inViewModel.ViewModelKey, addViewModelToGroupAction.ViewModelKey);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.RemoveViewModelFromGroupAction"/>
	/// </summary>
	[Fact]
	public void RemoveViewModelFromGroupAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var groupKey = Key<TextEditorGroup>.NewKey();

        var group = new TextEditorGroup(
            groupKey,
            inViewModel.ViewModelKey,
            new Key<TextEditorViewModel>[] { inViewModel.ViewModelKey }.ToImmutableList());

        var removeViewModelFromGroupAction = new TextEditorGroupState.RemoveViewModelFromGroupAction(
            group.GroupKey,
            inViewModel.ViewModelKey);

        Assert.Equal(groupKey, removeViewModelFromGroupAction.GroupKey);
        Assert.Equal(inViewModel.ViewModelKey, removeViewModelFromGroupAction.ViewModelKey);
	}

	/// <summary>
	/// <see cref="TextEditorGroupState.SetActiveViewModelOfGroupAction"/>
	/// </summary>
	[Fact]
	public void SetActiveViewModelOfGroupAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var groupKey = Key<TextEditorGroup>.NewKey();

        var group = new TextEditorGroup(
            groupKey,
            inViewModel.ViewModelKey,
            new Key<TextEditorViewModel>[] { inViewModel.ViewModelKey }.ToImmutableList());

        var setActiveViewModelOfGroupAction = new TextEditorGroupState.SetActiveViewModelOfGroupAction(
            group.GroupKey,
            inViewModel.ViewModelKey);

        Assert.Equal(groupKey, setActiveViewModelOfGroupAction.GroupKey);
        Assert.Equal(inViewModel.ViewModelKey, setActiveViewModelOfGroupAction.ViewModelKey);
	}
}