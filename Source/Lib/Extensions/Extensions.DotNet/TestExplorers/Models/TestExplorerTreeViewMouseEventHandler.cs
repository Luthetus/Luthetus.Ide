using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly LuthetusCommonApi _commonApi;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly ITextEditorService _textEditorService;
	private readonly IServiceProvider _serviceProvider;

	public TestExplorerTreeViewMouseEventHandler(
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

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewStringFragment treeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewMouseEventHandler),
				$"Could not open in editor because node is not type: {nameof(TreeViewStringFragment)}",
				_commonApi.ComponentRendererApi,
				_commonApi.NotificationApi,
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		if (treeViewStringFragment.Parent is not TreeViewStringFragment parentTreeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewMouseEventHandler),
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
			nameof(TestExplorerTreeViewMouseEventHandler),
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