using Fluxor;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

public partial record FindAllState
{
    public class Reducer
    {
        [ReducerMethod]
        public static FindAllState ReduceWithAction(
            FindAllState inState,
            WithAction withAction)
        {
            var outState = withAction.WithFunc.Invoke(inState);

            if (outState.Query.StartsWith("f:"))
            {
                outState = outState with
                {
                    FindAllFilterKind = Models.FindAllFilterKind.Files
                };
            }
            else if (outState.Query.StartsWith("t:"))
            {
                outState = outState with
                {
                    FindAllFilterKind = Models.FindAllFilterKind.Types
                };
            }
            else if (outState.Query.StartsWith("m:"))
            {
                outState = outState with
                {
                    FindAllFilterKind = Models.FindAllFilterKind.Members
                };
            }
            else
            {
                outState = outState with
                {
                    FindAllFilterKind = Models.FindAllFilterKind.None
                };
            }

            return outState;
        }

        [ReducerMethod]
        public static FindAllState ReduceAddResultAction(
            FindAllState inState,
            AddResultAction addResultAction)
        {
            return inState with
            {
                ResultList = inState.ResultList.Add(addResultAction.Result)
            };
        }

        [ReducerMethod(typeof(ClearResultListAction))]
        public static FindAllState ReduceClearResultListAction(
            FindAllState inState)
        {
            return inState with
            {
                ResultList = ImmutableList<string>.Empty
            };
        }
    }
}
