using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.States;

/// <summary>
/// <see cref="TextEditorDiffState.Reducer"/>
/// </summary>
public partial class TextEditorDiffStateReducerTests
{
	/// <summary>
	/// <see cref="TextEditorDiffState.Reducer.ReduceRegisterAction(TextEditorDiffState, TextEditorDiffState.RegisterAction)"/>
	/// </summary>
	[Fact]
	public void ReduceRegisterAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var outModelResourceUri = new ResourceUri("/unitTesting.txt_out");

        textEditorService.ModelApi.RegisterTemplated(
            ExtensionNoPeriodFacts.TXT,
            outModelResourceUri,
            DateTime.UtcNow,
            "Hello World!");

        _ = textEditorService.ModelApi.GetOrDefault(outModelResourceUri)
           ?? throw new ArgumentNullException();

        var outViewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            outViewModelKey,
            outModelResourceUri,
            new TextEditorCategory("UnitTesting"));

        var outViewModel = textEditorService.ViewModelApi.GetOrDefault(outViewModelKey)
           ?? throw new ArgumentNullException();

        var diffKey = new Key<TextEditorDiffModel>();

        var registerAction = new TextEditorDiffState.RegisterAction(
            diffKey,
            inViewModel.ViewModelKey,
            outViewModelKey);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorDiffStateWrap = serviceProvider.GetRequiredService<IState<TextEditorDiffState>>();

        Assert.Empty(textEditorDiffStateWrap.Value.DiffModelList);

        dispatcher.Dispatch(registerAction);
        Assert.Single(textEditorDiffStateWrap.Value.DiffModelList);

        var diffModel = textEditorDiffStateWrap.Value.DiffModelList.Single(x => x.DiffKey == diffKey);
        Assert.Equal(diffModel.DiffKey, registerAction.DiffKey);
        Assert.Equal(diffModel.InViewModelKey, registerAction.InViewModelKey);
        Assert.Equal(diffModel.OutViewModelKey, registerAction.OutViewModelKey);
    }

	/// <summary>
	/// <see cref="TextEditorDiffState.Reducer.ReduceDisposeAction(TextEditorDiffState, TextEditorDiffState.DisposeAction)"/>
	/// </summary>
	[Fact]
	public void ReduceDisposeAction()
	{
        TextEditorServicesTestsHelper.InitializeTextEditorServicesTestsHelper(
            out var textEditorService,
            out var inModel,
            out var inViewModel,
            out var serviceProvider);

        var outModelResourceUri = new ResourceUri("/unitTesting.txt_out");

        textEditorService.ModelApi.RegisterTemplated(
            ExtensionNoPeriodFacts.TXT,
            outModelResourceUri,
            DateTime.UtcNow,
            "Hello World!");

        _ = textEditorService.ModelApi.GetOrDefault(outModelResourceUri)
           ?? throw new ArgumentNullException();

        var outViewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            outViewModelKey,
            outModelResourceUri,
            new TextEditorCategory("UnitTesting"));

        var outViewModel = textEditorService.ViewModelApi.GetOrDefault(outViewModelKey)
           ?? throw new ArgumentNullException();

        var diffKey = new Key<TextEditorDiffModel>();

        var registerAction = new TextEditorDiffState.RegisterAction(
            diffKey,
            inViewModel.ViewModelKey,
            outViewModelKey);

        var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        var textEditorDiffStateWrap = serviceProvider.GetRequiredService<IState<TextEditorDiffState>>();

        dispatcher.Dispatch(registerAction);
        Assert.Single(textEditorDiffStateWrap.Value.DiffModelList);

        dispatcher.Dispatch(new TextEditorDiffState.DisposeAction(diffKey));
        Assert.Empty(textEditorDiffStateWrap.Value.DiffModelList);
    }
}