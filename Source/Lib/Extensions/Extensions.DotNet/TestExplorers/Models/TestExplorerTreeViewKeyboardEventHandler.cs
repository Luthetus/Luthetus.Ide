using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
	private readonly LuthetusCommonApi _commonApi;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly ITextEditorService _textEditorService;
	private readonly IServiceProvider _serviceProvider;

	public TestExplorerTreeViewKeyboardEventHandler(
			LuthetusCommonApi commonApi,
			ICompilerServiceRegistry compilerServiceRegistry,
			ITextEditorService textEditorService,
			IServiceProvider serviceProvider)
		: base(commonApi)
	{
		_commonApi = commonApi;
		_compilerServiceRegistry = compilerServiceRegistry;
		_textEditorService = textEditorService;
		_serviceProvider = serviceProvider;
	}

	public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
	{
		if (commandArgs.KeyboardEventArgs is null)
			return Task.CompletedTask;

		base.OnKeyDownAsync(commandArgs);

		switch (commandArgs.KeyboardEventArgs.Code)
		{
			case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
				return InvokeOpenInEditor(commandArgs, true);
			case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
				return InvokeOpenInEditor(commandArgs, false);
		}

		return Task.CompletedTask;
	}

	private Task InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
	{
		var activeNode = commandArgs.TreeViewContainer.ActiveNode;

		if (activeNode is not TreeViewStringFragment treeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewKeyboardEventHandler),
				$"Could not open in editor because node is not type: {nameof(TreeViewStringFragment)}",
				_commonApi.ComponentRendererApi,
				_commonApi.NotificationApi,
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		if (treeViewStringFragment.Parent is not TreeViewStringFragment parentTreeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewKeyboardEventHandler),
				$"Could not open in editor because node's parent does not seem to include a class name",
				_commonApi.ComponentRendererApi,
				_commonApi.NotificationApi,
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		var className = parentTreeViewStringFragment.Item.Value.Split('.').Last();

		NotificationHelper.DispatchInformative(
			nameof(TestExplorerTreeViewMouseEventHandler),
			className + ".cs",
			_commonApi.ComponentRendererApi,
			_commonApi.NotificationApi,
			TimeSpan.FromSeconds(5));

		var methodName = treeViewStringFragment.Item.Value.Trim();

		NotificationHelper.DispatchInformative(
			nameof(TestExplorerTreeViewMouseEventHandler),
			methodName + "()",
			_commonApi.ComponentRendererApi,
			_commonApi.NotificationApi,
			TimeSpan.FromSeconds(5));

		_textEditorService.TextEditorWorker.PostUnique(
			nameof(TestExplorerTreeViewKeyboardEventHandler),
			TestExplorerHelper.ShowTestInEditorFactory(
				className,
				methodName,
				_commonApi.ComponentRendererApi,
				_commonApi.NotificationApi,
				_compilerServiceRegistry,
				_textEditorService,
				_serviceProvider));

		return Task.CompletedTask;
	}
}