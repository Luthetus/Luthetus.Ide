using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageState
{
    public record LocalStorageSetItemTask(LocalStorageSync Sync, string Key, string Value);
}
