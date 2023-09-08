using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Fluxor;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;

[FeatureState]
public partial record CompilerServiceExplorerState(
    IAbsoluteFilePath? AbsoluteFilePath,
    bool IsLoadingCompilerServiceExplorer)
{
    public static readonly TreeViewStateKey TreeViewCompilerServiceExplorerContentStateKey = TreeViewStateKey.NewTreeViewStateKey();

    private CompilerServiceExplorerState() : this(
        default,
        false)
    {

    }
}