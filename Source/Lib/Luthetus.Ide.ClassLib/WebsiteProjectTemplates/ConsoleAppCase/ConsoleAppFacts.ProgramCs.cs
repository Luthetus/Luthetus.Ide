using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.ConsoleAppCase;

public partial class ConsoleAppFacts
{
    public const string PROGRAM_CS_RELATIVE_FILE_PATH = @"Program.cs";

    public static string GetProgramCsContents(string projectName) => @$"// See https://aka.ms/new-console-template for more information
Console.WriteLine(""Hello, World!"");
";
}
