using Luthetus.Common.RazorLib.PolymorphicViewModels.States;

namespace Luthetus.TextEditor.RazorLib.PolymorphicViewModels.States;

public partial record PolymorphicViewModelState
{
	public static class Reducer
	{
		[ReducerMethod]
		public static PolymorphicUiState ReduceRegisterAction(
			PolymorphicUiState inPolymorphicUiState,
			RegisterAction registerAction)
		{
			return inPolymorphicUiState with
			{
				Map = inPolymorphicUiState.Map.Add(
					registerAction.FullTypeName,
					registerAction.PolymorphicUiRecord);
			};
		}

		[ReducerMethod]
		public static PolymorphicUiState ReduceDisposeAction(
			PolymorphicUiState inPolymorphicUiState,
			RegisterAction registerAction)
		{
			return inPolymorphicUiState with
			{
				Map = inPolymorphicUiState.Map.Remove(registerAction.FullTypeName);
			};
		}
	}
}
