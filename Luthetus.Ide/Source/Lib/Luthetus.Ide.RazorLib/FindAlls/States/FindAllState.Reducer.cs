using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return withAction.withFunc.Invoke(inState);
        }
    }
}
