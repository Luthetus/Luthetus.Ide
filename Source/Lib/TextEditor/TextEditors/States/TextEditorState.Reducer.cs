using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;

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
            if (inState.ModelList.Any(x => x.ResourceUri == registerModelAction.Model.ResourceUri))
                return inState;

            var outModelList = inState.ModelList.Add(registerModelAction.Model);

            return inState with { ModelList = outModelList };
        }

        [ReducerMethod]
        public static TextEditorState ReduceDisposeModelAction(
            TextEditorState inState,
            DisposeModelAction disposeModelAction)
        {
            var inModel = inState.ModelList.SingleOrDefault(
                x => x.ResourceUri == disposeModelAction.ResourceUri);

            if (inModel is null)
                return inState;

            var outModelList = inState.ModelList.Remove(inModel);

            return inState with { ModelList = outModelList };
        }

        [ReducerMethod]
        public static TextEditorState ReduceSetModelAction(
            TextEditorState inState,
            SetModelAction setModelAction)
        {
            var inModel = inState.ModelList.FirstOrDefault(
                x => x.ResourceUri == setModelAction.ModelModifier.ResourceUri);

            if (inModel is null)
                return inState;

            var outViewModelList = inState.ModelList.Replace(
                inModel,
                setModelAction.ModelModifier.ToModel());

            return inState with { ModelList = outViewModelList };
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
        	
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == registerViewModelAction.ViewModelKey);

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
                VirtualizationResult<List<RichCharacter>>.GetEmptyRichCharacters(),
				new TextEditorDimensions(0, 0, 0, 0),
				new ScrollbarDimensions(0, 0, 0, 0, 0),
        		new CharAndLineMeasurements(0, 0),
                false,
                registerViewModelAction.Category);

            var outViewModelList = inState.ViewModelList.Add(viewModel);

            return inState with { ViewModelList = outViewModelList };
        }
        
        [ReducerMethod]
        public static TextEditorState ReduceRegisterViewModelExistingAction(
            TextEditorState inState,
            RegisterViewModelExistingAction registerViewModelExistingAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == registerViewModelExistingAction.ViewModel.ViewModelKey);

            if (inViewModel is not null)
                return inState;

            if (registerViewModelExistingAction.ViewModel.ViewModelKey == Key<TextEditorViewModel>.Empty)
                throw new InvalidOperationException($"Provided {nameof(Key<TextEditorViewModel>)} cannot be {nameof(Key<TextEditorViewModel>)}.{Key<TextEditorViewModel>.Empty}");

            var outViewModelList = inState.ViewModelList.Add(registerViewModelExistingAction.ViewModel);

            return inState with { ViewModelList = outViewModelList };
        }

        [ReducerMethod]
        public static TextEditorState ReduceDisposeViewModelAction(
            TextEditorState inState,
            DisposeViewModelAction disposeViewModelAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == disposeViewModelAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModelList = inState.ViewModelList.Remove(inViewModel);
			
            inViewModel.Dispose();

            return inState with { ViewModelList = outViewModelList };
        }

        [ReducerMethod]
        public static TextEditorState ReduceSetViewModelWithAction(
            TextEditorState inState,
            SetViewModelWithAction setViewModelWithAction)
        {
            var inViewModel = inState.ViewModelList.FirstOrDefault(
                x => x.ViewModelKey == setViewModelWithAction.ViewModelKey);

            if (inViewModel is null)
                return inState;

            var outViewModel = setViewModelWithAction.WithFunc.Invoke(inViewModel);
            var outViewModelList = inState.ViewModelList.Replace(inViewModel, outViewModel);

            return inState with { ViewModelList = outViewModelList };
        }

		[ReducerMethod]
        public static TextEditorState ReduceSetModelAndViewModelRangeAction(
            TextEditorState inState,
            SetModelAndViewModelRangeAction setModelAndViewModelRangeAction)
        {
			var mutableModelList = new List<TextEditorModel>(inState.ModelList);
			var mutableViewModelList = new List<TextEditorViewModel>(inState.ViewModelList);

			// Models
			foreach (var modelModifier in setModelAndViewModelRangeAction.ModelModifierList)
			{
				var indexExistingModel = mutableModelList.FindIndex(
	                x => x.ResourceUri == modelModifier.ResourceUri);
	
				if (indexExistingModel != -1)
					mutableModelList[indexExistingModel] = modelModifier.ToModel();
			}

			// ViewModels
			foreach (var viewModelModifier in setModelAndViewModelRangeAction.ViewModelModifierList)
			{
				var indexExistingViewModel = mutableViewModelList.FindIndex(
	                x => x.ViewModelKey == viewModelModifier.ViewModel.ViewModelKey);
	
	            if (indexExistingViewModel != -1)
	                mutableViewModelList[indexExistingViewModel] = viewModelModifier.ViewModel;
			}

            return inState with
			{
				ModelList = mutableModelList.ToImmutableList(),
				ViewModelList = mutableViewModelList.ToImmutableList(),
			};
        }
	}
}
