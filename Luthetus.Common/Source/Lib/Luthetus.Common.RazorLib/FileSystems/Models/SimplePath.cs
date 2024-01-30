using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class SimplePath
{
    public SimplePath(string absolutePath, bool isDirectory)
    {
        AbsolutePath = absolutePath;
        IsDirectory = isDirectory;
    }

    public string AbsolutePath { get; }
    public bool IsDirectory { get; }
}
