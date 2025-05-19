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
			for (int i = 0; i < viewModel.InlineUiList.Count; i++)
			{
				var inlineUiTuple = viewModel.InlineUiList[i];
				
				if (initialCursorPositionIndex <= inlineUiTuple.InlineUi.PositionIndex)
				{
					if (viewModel.PersistentState.VirtualAssociativityKind == VirtualAssociativityKind.Right)
						continue;
				
					inlineUiTuple.InlineUi = viewModel.InlineUiList[i].InlineUi.WithIncrementPositionIndex(insertionLength);
					viewModel.InlineUiList[i] = inlineUiTuple;
				}
			}
			
			if (lineEndPositionWasAdded && viewModel.PersistentState.DisplayTracker.ComponentData is not null)
			{
				viewModel.PersistentState.DisplayTracker.ComponentData.Virtualized_LineIndexCache_IsInvalid = true;
			}
			else
			{
				viewModel.PersistentState.DisplayTracker.ComponentData.Virtualized_LineIndexCache_LineIndexWithModificationList.Add(initialCursorLineIndex);
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
			for (int i = 0; i < viewModel.InlineUiList.Count; i++)
			{
				var inlineUiTuple = viewModel.InlineUiList[i];
				
				if (endExclusiveIndex - 1 < inlineUiTuple.InlineUi.PositionIndex)
				{
					inlineUiTuple.InlineUi = viewModel.InlineUiList[i].InlineUi.WithDecrementPositionIndex(endExclusiveIndex - startInclusiveIndex);
					viewModel.InlineUiList[i] = inlineUiTuple;
				}
			}
			
			if (lineEndPositionWasAdded && viewModel.PersistentState.DisplayTracker.ComponentData is not null)
			{
				viewModel.PersistentState.DisplayTracker.ComponentData.Virtualized_LineIndexCache_IsInvalid = true;
			}
			else
			{
				viewModel.PersistentState.DisplayTracker.ComponentData.Virtualized_LineIndexCache_LineIndexWithModificationList.Add(initialCursorLineIndex);
			}
		}
	}
	
	public void SetContent(List<Key<TextEditorViewModel>> viewModelKeyList)
	{
		var editContext = new TextEditorEditContext(_textEditorService);
		
		foreach (var viewModelKey in viewModelKeyList)
		{
			var viewModel = editContext.GetViewModelModifier(viewModelKey);
			
			if (viewModel.PersistentState.DisplayTracker.ComponentData is not null)
				viewModel.PersistentState.DisplayTracker.ComponentData.Virtualized_LineIndexCache_IsInvalid = true;
		}
	}
}
