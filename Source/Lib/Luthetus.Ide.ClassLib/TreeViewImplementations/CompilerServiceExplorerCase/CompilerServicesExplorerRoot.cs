using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.CompilerServiceExplorerCase;

public class CompilerServicesExplorerRoot
{
    public CompilerServicesExplorerRoot(CSharpCompilerService cSharpCompilerService)
    {
        CSharpCompilerService = cSharpCompilerService;
    }

    public CSharpCompilerService CSharpCompilerService { get; }
}
