using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.States;

public partial record TextEditorState
{
	public class Reducer
	{
		[ReducerMethod]
        public static TextEditorState ReduceRegisterModelAction(
            TextEditorState inState,
            RegisterModelAction registerModelAction)
        {
        	var exists = inState._modelMap.TryGetValue(
        		registerModelAction.Model.ResourceUri, out var inModel);
        	
        	if (exists)
                return inState;
			
			inState._modelMap.Add(
        		registerModelAction.Model.ResourceUri, registerModelAction.Model);
        		
        	return inState with {};
        }

        [ReducerMethod]
        public static TextEditorState ReduceDisposeModelAction(
            TextEditorState inState,
            DisposeModelAction disposeModelAction)
        {
        	var exists = inState._modelMap.TryGetValue(
        		disposeModelAction.ResourceUri, out var inModel);

            if (!exists)
                return inState;

			inState._modelMap.Remove(disposeModelAction.ResourceUri);
			
			return inState with {};
        }

        [ReducerMethod]
        public static TextEditorState ReduceSetModelAction(
            TextEditorState inState,
            SetModelAction setModelAction)
        {
        	var exists = inState._modelMap.TryGetValue(
        		setModelAction.ModelModifier.ResourceUri, out var inModel);

            if (!exists)
                return inState;

			inState._modelMap[inModel.ResourceUri] = setModelAction.ModelModifier.ToModel();
        	
        	return inState with {};
        }

		[ReducerMethod]
        public static TextEditorState ReduceRegisterViewModelAction(
            TextEditorState inState,
            RegisterViewModelAction registerViewModelAction)
        {
        	// The category and ViewModelKey do NOT need to be a compound unique identifier
        	// Only check for the 'ViewModelKey' already existing.
        	//
        	// Category is just a way to filter a list of view models.
        	//
        	// TODO: What is the difference between Category and Group? I'm asking this to myself. If their redundant then get rid of one. Otherwise...
        	//       ...write down the justification for both existing before you forget again.
        	//
        	// I think I made both Category and Group because:
        	// Category describes relationships between view models
        	//
        	// Group is solely meant to provide tab UI.
        	// 	- One can put a view model of any category into a group.
        	// 	- Or one could add dropzone logic that validates the category of a 'being dragged view model'
        	//     	  to ensure it belongs in that group.
        	
            var inViewModel = inState.ViewModelGetOrDefault(registerViewModelAction.ViewModelKey);

            if (inViewModel is not null)
                return inState;

            if (registerViewModelAction.ViewModelKey == Key<TextEditorViewModel>.Empty)
                throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

            var viewModel = new TextEditorViewModel(
                registerViewModelAction.ViewModelKey,
                registerViewModelAction.ResourceUri,
                registerViewModelAction.TextEditorService,
                registerViewModelAction.Dispatcher,
                registerViewModelAction.DialogService,
                registerViewModelAction.JsRuntime,
                VirtualizationGrid.Empty,
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
        		new CharAndLineMeasurements(0, 0),
                false,
                registerViewModelAction.Category);

			inState._viewModelMap.Add(viewModel.ViewModelKey, viewModel);
        	
        	return inState with {};
        }
        
        [ReducerMethod]
        public static TextEditorState ReduceRegisterViewModelExistingAction(
            TextEditorState inState,
            RegisterViewModelExistingAction registerViewModelExistingAction)
        {
            var inViewModel = inState.ViewModelGetOrDefault(
            	registerViewModelExistingAction.ViewModel.ViewModelKey);

            if (inViewModel is not null)
                return inState;

            if (registerViewModelExistingAction.ViewModel.ViewModelKey == Key<TextEditorViewModel>.Empty)
                throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

			inState._viewModelMap.Add(
				registerViewModelExistingAction.ViewModel.ViewModelKey,
				registerViewModelExistingAction.ViewModel);
				
        	return inState with {};
        }

        [ReducerMethod]
        public static TextEditorState ReduceDisposeViewModelAction(
            TextEditorState inState,
            DisposeViewModelAction disposeViewModelAction)
        {
            var inViewModel = inState.ViewModelGetOrDefault(
                disposeViewModelAction.ViewModelKey);

            if (inViewModel is null)
                return inState;
                
			inState._viewModelMap.Remove(inViewModel.ViewModelKey);
			inViewModel.Dispose();
        	return inState with {};
        }

        [ReducerMethod]
        public static TextEditorState ReduceSetViewModelWithAction(
            TextEditorState inState,
            SetViewModelWithAction setViewModelWithAction)
        {
            var inViewModel = inState.ViewModelGetOrDefault(
                setViewModelWithAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

			var outViewModel = setViewModelWithAction.WithFunc.Invoke(inViewModel);
            inState._viewModelMap[inViewModel.ViewModelKey] = outViewModel;
            return inState with {};
        }

		[ReducerMethod]
        public static TextEditorState ReduceSetModelAndViewModelRangeAction(
            TextEditorState inState,
            SetModelAndViewModelRangeAction setModelAndViewModelRangeAction)
        {
    		// Models
			foreach (var modelModifier in setModelAndViewModelRangeAction.ModelModifierList)
			{
				// Enumeration was modified shouldn't occur here because only the reducer
				// should be adding or removing, and the reducer is thread safe.
				var exists = inState._modelMap.TryGetValue(
	        		modelModifier.ResourceUri, out var inModel);
	
	            if (!exists)
	                continue;
	                
				inState._modelMap[modelModifier.ResourceUri] = modelModifier.ToModel();
			}
			
			// ViewModels
			foreach (var viewModelModifier in setModelAndViewModelRangeAction.ViewModelModifierList)
			{
				// Enumeration was modified shouldn't occur here because only the reducer
				// should be adding or removing, and the reducer is thread safe.
				var exists = inState._viewModelMap.TryGetValue(
	        		viewModelModifier.ViewModel.ViewModelKey, out var inViewModel);
	        		
	        	if (!exists)
	                continue;
	
                inState._viewModelMap[viewModelModifier.ViewModel.ViewModelKey] = viewModelModifier.ViewModel;
			}

            return inState with {};
        }
	}
}
