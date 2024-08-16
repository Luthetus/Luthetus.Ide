using System.Text;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public class OutputTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly ITextEditorService _textEditorService;
	private readonly LuthetusTextEditorConfig _textEditorConfig;
	private readonly IServiceProvider _serviceProvider;

	public OutputTreeViewMouseEventHandler(
			ITextEditorService textEditorService,
			LuthetusTextEditorConfig textEditorConfig,
			IServiceProvider serviceProvider,
			ITreeViewService treeViewService,
			IBackgroundTaskService backgroundTaskService)
		: base(treeViewService, backgroundTaskService)
	{
		_textEditorService = textEditorService;
		_textEditorConfig = textEditorConfig;
		_serviceProvider = serviceProvider;
	}

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewDiagnosticLine treeViewDiagnosticLine)
			return Task.CompletedTask;
			
		var lineAndColumnIndicesString = treeViewDiagnosticLine.Item.LineAndColumnIndicesTextSpan.Text;
		var position = 0;
		
		int? lineIndex = null;
		int? columnIndex = null;
		
		while (position < lineAndColumnIndicesString.Length)
		{
			var character = lineAndColumnIndicesString[position];
		
			if (char.IsDigit(character))
			{
				var numberBuilder = new StringBuilder(character);
				
				while (true)
				{
					character = lineAndColumnIndicesString[position];
					
					if (char.IsDigit(character))
						numberBuilder.Append(character);
					else
						break;
					
					position++;
				}
				
				if (int.TryParse(numberBuilder.ToString(), out var number))
				{
					if (lineIndex is null)
						lineIndex = number;
					else if (columnIndex is null)
						columnIndex = number;
				}
			}
			
			position++;
		}

		return _textEditorService.OpenInEditorAsync(
			treeViewDiagnosticLine.Item.FilePathTextSpan.Text,
			true,
			0,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
	}
}
