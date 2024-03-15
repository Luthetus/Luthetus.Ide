namespace Luthetus.Common.RazorLib.PolymorphicViewModels.States;

public partial record PolymorphicViewModelState
{
	public static class Reducer
	{
		[ReducerMethod]
		public static PolymorphicViewModelState ReduceRegisterAction(
			PolymorphicViewModelState inPolymorphicViewModelState,
			RegisterAction registerAction)
		{
			return inPolymorphicViewModelState with
			{
				Map = inPolymorphicViewModelState.Map.Add(
					registerAction.FullTypeName,
					registerAction.PolymorphicUiRecord)
			};
		}

		[ReducerMethod]
		public static PolymorphicViewModelState ReduceDisposeAction(
			PolymorphicViewModelState inPolymorphicViewModelState,
			RegisterAction registerAction)
		{
			return inPolymorphicViewModelState with
			{
				Map = inPolymorphicViewModelState.Map.Remove(registerAction.FullTypeName)
			};
		}
	}
}
