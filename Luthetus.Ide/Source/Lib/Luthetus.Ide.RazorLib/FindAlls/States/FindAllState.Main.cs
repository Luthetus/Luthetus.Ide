using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.FindAlls.States;

[FeatureState]
public partial record FindAllState(string Query)
{
    public FindAllState() : this(string.Empty)
    {
    }
}
