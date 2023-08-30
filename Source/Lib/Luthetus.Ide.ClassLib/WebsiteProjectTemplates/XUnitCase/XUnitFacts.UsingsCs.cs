using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.XUnitCase;

public partial class XUnitFacts
{
    public const string USINGS_CS_RELATIVE_FILE_PATH = @"Usings.cs";

    public static string GetUsingsCsContents(string projectName) => @$"global using Xunit;";
}
