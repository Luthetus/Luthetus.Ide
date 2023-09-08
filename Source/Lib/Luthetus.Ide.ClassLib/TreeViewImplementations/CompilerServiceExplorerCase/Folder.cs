using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.CompilerServiceExplorerCase;

public class Folder
{
    public Folder(string displayName, Func<Task<List<TreeViewNoType>>> loadAsyncTask)
    {
        DisplayName = displayName;
        LoadAsyncTask = loadAsyncTask;
    }

    public string DisplayName { get; }
    public Func<Task<List<TreeViewNoType>>> LoadAsyncTask { get; }
}
