using System.Collections.Immutable;
using Fluxor;
using Luthetus.Ide.RazorLib.Shareds.Models;

namespace Luthetus.Ide.RazorLib.Shareds.States;

[FeatureState]
public record IdeMainLayoutState(ImmutableList<FooterJustifyEndComponent> FooterJustifyEndComponentList)
{
	public IdeMainLayoutState() : this(ImmutableList<FooterJustifyEndComponent>.Empty)
	{
	}

	public record RegisterFooterJustifyEndComponentAction(FooterJustifyEndComponent FooterJustifyEndComponent);

	public class Reducer
	{
		[ReducerMethod]
		public static IdeMainLayoutState ReduceRegisterFooterJustifyEndComponentAction(
			IdeMainLayoutState inState,
			RegisterFooterJustifyEndComponentAction registerFooterJustifyEndComponentAction)
		{
			var existingComponent = inState.FooterJustifyEndComponentList.FirstOrDefault(x =>
				x.Key == registerFooterJustifyEndComponentAction.FooterJustifyEndComponent.Key);
				
			if (existingComponent is not null)
				return inState;
		
			return inState with
			{
				FooterJustifyEndComponentList = inState.FooterJustifyEndComponentList.Add(
					registerFooterJustifyEndComponentAction.FooterJustifyEndComponent)
			};
		}
	}
}
