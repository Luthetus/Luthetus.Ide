using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

public partial record FindAllState
{
    public record WithAction(Func<FindAllState, FindAllState> withFunc);
}
