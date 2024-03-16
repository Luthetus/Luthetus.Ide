using Fluxor;

namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

public partial record PolymorphicViewModelState
{
	public static class Reducer
	{
		[ReducerMethod]
		public static PolymorphicViewModelState ReduceRegisterAction(
			PolymorphicViewModelState inState,
			RegisterAction registerAction)
		{
			if (inState.Map.ContainsKey(registerAction.Guid))
				return inState;
			
			return inState with
			{
				Map = inState.Map.Add(registerAction.Guid, registerAction.PolymorphicViewModel)
			};
		}

		[ReducerMethod]
		public static PolymorphicViewModelState ReduceDisposeAction(
			PolymorphicViewModelState inState,
			DisposeAction disposeAction)
		{
			if (!inState.Map.ContainsKey(disposeAction.Guid))
				return inState;
			
			return inState with
			{
				Map = inState.Map.Remove(disposeAction.Guid)
			};
		}
	}
}
