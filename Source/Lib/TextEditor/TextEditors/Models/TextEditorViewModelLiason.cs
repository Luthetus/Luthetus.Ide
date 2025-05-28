using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelLiason
{
	private readonly TextEditorService _textEditorService;
	
	public TextEditorViewModelLiason(TextEditorService textEditorService)
	{
		_textEditorService = textEditorService;
	}

	/// <summary>
	/// 'TextEditorEditContext' is more-so just a way to indicate thread safety
	/// for a given method.
	///
	/// It doesn't actually store anything, all the state is still on the ITextEditorService.
	///
	/// This method 'InsertRepositionInlineUiList(...)' is quite deep inside a chain of calls,
	/// and this method is meant for internal use.
	///
	/// Therefore, I'm going to construct the 'TextEditorEditContext' out of thin air.
	/// But, everything will still work because 'TextEditorEditContext' never actually stored anything.
	///
	/// I need the 'TextEditorEditContext' because if they have a pending edit on the viewmodel
	/// that I'm about to reposition the InlineUiList for, then everything will get borked.
	///
	/// This will get me their pending edit if it exists, otherwise it will start a pending edit.
	/// </summary>
	public void InsertRepositionInlineUiList(
		int initialCursorPositionIndex,
		int insertionLength,
		List<Key<TextEditorViewModel>> viewModelKeyList,
		int initialCursorLineIndex,
		bool lineEndPositionWasAdded)
	{
		var editContext = new TextEditorEditContext(_textEditorService);
		
		foreach (var viewModelKey in viewModelKeyList)
		{
			var viewModel = editContext.GetViewModelModifier(viewModelKey);
			
			// TODO: Determine which viewmodel sent the event?
			for (int i = 0; i < viewModel.PersistentState.InlineUiList.Count; i++)
			{
				var inlineUiTuple = viewModel.PersistentState.InlineUiList[i];
				
				if (initialCursorPositionIndex <= inlineUiTuple.InlineUi.PositionIndex)
				{
					if (viewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.Right)
						continue;
				
					inlineUiTuple.InlineUi = viewModel.PersistentState.InlineUiList[i].InlineUi.WithIncrementPositionIndex(insertionLength);
					viewModel.PersistentState.InlineUiList[i] = inlineUiTuple;
				}
			}
			
			/*
				This error shouldn't be possible because you can only dispose the ComponentData
				from within the editContext but nevertheless I got an NRE so I'm capturing the reference before null check.
			
				ERROR on (backgroundTask.Name was here): System.NullReferenceException: Object reference not set to an instance of an object.
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorViewModelLiason.InsertRepositionInlineUiList(Int32 initialCursorPositionIndex, Int32 insertionLength, List`1 viewModelKeyList, Int32 initialCursorLineIndex, Boolean lineEndPositionWasAdded) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorViewModelLiason.cs:line 65
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.InsertMetadata(String value, TextEditorViewModel viewModel) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1273
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.Insert(String value, TextEditorViewModel viewModel, Boolean shouldCreateEditHistory) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1063
					   at Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults.TextEditorKeymapDefault.HandleEvent(TextEditorComponentData componentData, Key`1 viewModelKey, KeyboardEventArgs keyboardEventArgs) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\Keymaps\Models\Defaults\TextEditorKeymapDefault.cs:line 595
					   at Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorWorkerUi.HandleEvent() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\BackgroundTasks\Models\TextEditorWorkerUi.cs:line 123
					   at Luthetus.Common.RazorLib.BackgroundTasks.Models.ContinuousBackgroundTaskWorker.ExecuteAsync(CancellationToken cancellationToken) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Common\BackgroundTasks\Models\ContinuousBackgroundTaskWorker.cs:line 41
					PS C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\bin\Release\net8.0\publish>
			*/
			var componentData = viewModel.PersistentState.ComponentData;
			if (lineEndPositionWasAdded && componentData is not null)
			{
				componentData.Virtualized_LineIndexCache_IsInvalid = true;
			}
			else
			{
				componentData.Virtualized_LineIndexCache_LineIndexWithModificationList.Add(initialCursorLineIndex);
			}
		}
	}
	
	/// <summary>
	/// See: 'InsertRepositionInlineUiList(...)' summary
	///      for 'TextEditorEditContext' explanation.
	/// </summary>
	public void DeleteRepositionInlineUiList(
		int startInclusiveIndex,
		int endExclusiveIndex,
		List<Key<TextEditorViewModel>> viewModelKeyList,
		int initialCursorLineIndex,
		bool lineEndPositionWasAdded)
	{
		var editContext = new TextEditorEditContext(_textEditorService);
		
		foreach (var viewModelKey in viewModelKeyList)
		{
			var viewModel = editContext.GetViewModelModifier(viewModelKey);
			
			// TODO: Determine which viewmodel sent the event?
			for (int i = 0; i < viewModel.PersistentState.InlineUiList.Count; i++)
			{
				var inlineUiTuple = viewModel.PersistentState.InlineUiList[i];
				
				if (endExclusiveIndex - 1 < inlineUiTuple.InlineUi.PositionIndex)
				{
					inlineUiTuple.InlineUi = viewModel.PersistentState.InlineUiList[i].InlineUi.WithDecrementPositionIndex(endExclusiveIndex - startInclusiveIndex);
					viewModel.PersistentState.InlineUiList[i] = inlineUiTuple;
				}
			}
			
			/*
				This error shouldn't be possible because you can only dispose the ComponentData
				from within the editContext but nevertheless I got an NRE so I'm capturing the reference before null check.
			
				ERROR on (backgroundTask.Name was here): System.NullReferenceException: Object reference not set to an instance of an object.
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorViewModelLiason.InsertRepositionInlineUiList(Int32 initialCursorPositionIndex, Int32 insertionLength, List`1 viewModelKeyList, Int32 initialCursorLineIndex, Boolean lineEndPositionWasAdded) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorViewModelLiason.cs:line 65
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.InsertMetadata(String value, TextEditorViewModel viewModel) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1273
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.Insert(String value, TextEditorViewModel viewModel, Boolean shouldCreateEditHistory) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1063
					   at Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults.TextEditorKeymapDefault.HandleEvent(TextEditorComponentData componentData, Key`1 viewModelKey, KeyboardEventArgs keyboardEventArgs) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\Keymaps\Models\Defaults\TextEditorKeymapDefault.cs:line 595
					   at Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorWorkerUi.HandleEvent() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\BackgroundTasks\Models\TextEditorWorkerUi.cs:line 123
					   at Luthetus.Common.RazorLib.BackgroundTasks.Models.ContinuousBackgroundTaskWorker.ExecuteAsync(CancellationToken cancellationToken) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Common\BackgroundTasks\Models\ContinuousBackgroundTaskWorker.cs:line 41
					PS C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\bin\Release\net8.0\publish>
			*/
			var componentData = viewModel.PersistentState.ComponentData;
			if (lineEndPositionWasAdded && componentData is not null)
			{
				componentData.Virtualized_LineIndexCache_IsInvalid = true;
			}
			else
			{
				componentData.Virtualized_LineIndexCache_LineIndexWithModificationList.Add(initialCursorLineIndex);
			}
		}
	}
	
	public void SetContent(List<Key<TextEditorViewModel>> viewModelKeyList)
	{
		var editContext = new TextEditorEditContext(_textEditorService);
		
		foreach (var viewModelKey in viewModelKeyList)
		{
			var viewModel = editContext.GetViewModelModifier(viewModelKey);
			
			/*
				This error shouldn't be possible because you can only dispose the ComponentData
				from within the editContext but nevertheless I got an NRE so I'm capturing the reference before null check.
			
				ERROR on (backgroundTask.Name was here): System.NullReferenceException: Object reference not set to an instance of an object.
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorViewModelLiason.InsertRepositionInlineUiList(Int32 initialCursorPositionIndex, Int32 insertionLength, List`1 viewModelKeyList, Int32 initialCursorLineIndex, Boolean lineEndPositionWasAdded) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorViewModelLiason.cs:line 65
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.InsertMetadata(String value, TextEditorViewModel viewModel) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1273
					   at Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModel.Insert(String value, TextEditorViewModel viewModel, Boolean shouldCreateEditHistory) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\TextEditors\Models\TextEditorModel.cs:line 1063
					   at Luthetus.TextEditor.RazorLib.Keymaps.Models.Defaults.TextEditorKeymapDefault.HandleEvent(TextEditorComponentData componentData, Key`1 viewModelKey, KeyboardEventArgs keyboardEventArgs) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\Keymaps\Models\Defaults\TextEditorKeymapDefault.cs:line 595
					   at Luthetus.TextEditor.RazorLib.BackgroundTasks.Models.TextEditorWorkerUi.HandleEvent() in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\TextEditor\BackgroundTasks\Models\TextEditorWorkerUi.cs:line 123
					   at Luthetus.Common.RazorLib.BackgroundTasks.Models.ContinuousBackgroundTaskWorker.ExecuteAsync(CancellationToken cancellationToken) in C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Common\BackgroundTasks\Models\ContinuousBackgroundTaskWorker.cs:line 41
					PS C:\Users\hunte\Repos\Luthetus.Ide_Fork\Source\Lib\Ide\Host.Photino\bin\Release\net8.0\publish>
			*/
			var componentData = viewModel.PersistentState.ComponentData;
			if (componentData is not null)
				componentData.Virtualized_LineIndexCache_IsInvalid = true;
		}
	}
}
