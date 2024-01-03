using Xunit;
using Luthetus.TextEditor.RazorLib.Diffs.States;
using Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorServices;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;

namespace Luthetus.TextEditor.Tests.Basis.Diffs.States;

/// <summary>
/// <see cref="TextEditorDiffState"/>
/// </summary>
public partial class TextEditorDiffStateActionsTests
{
	/// <summary>
	/// <see cref="TextEditorDiffState.RegisterAction"/>
	/// </summary>
	[Fact]
	public void RegisterAction()
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

        var outModel = textEditorService.ModelApi.GetOrDefault(outModelResourceUri)
           ?? throw new ArgumentNullException();

        var outViewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            outViewModelKey,
            outModelResourceUri);

        var outViewModel = textEditorService.ViewModelApi.GetOrDefault(outViewModelKey)
           ?? throw new ArgumentNullException();

        var diffKey = new Key<TextEditorDiffModel>();

        var registerAction = new TextEditorDiffState.RegisterAction(
            diffKey,
            inViewModel.ViewModelKey,
            outViewModelKey);

        Assert.Equal(diffKey, registerAction.DiffKey);
        Assert.Equal(inViewModel.ViewModelKey, registerAction.InViewModelKey);
        Assert.Equal(outViewModel.ViewModelKey, registerAction.OutViewModelKey);
	}

	/// <summary>
	/// <see cref="TextEditorDiffState.DisposeAction"/>
	/// </summary>
	[Fact]
	public void DisposeAction()
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

        var outModel = textEditorService.ModelApi.GetOrDefault(outModelResourceUri)
           ?? throw new ArgumentNullException();

        var outViewModelKey = Key<TextEditorViewModel>.NewKey();

        textEditorService.ViewModelApi.Register(
            outViewModelKey,
            outModelResourceUri);

        var outViewModel = textEditorService.ViewModelApi.GetOrDefault(outViewModelKey)
           ?? throw new ArgumentNullException();

        var diffKey = new Key<TextEditorDiffModel>();

        var disposeAction = new TextEditorDiffState.DisposeAction(diffKey);

        Assert.Equal(diffKey, disposeAction.DiffKey);
	}
}