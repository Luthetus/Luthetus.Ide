namespace Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

public partial class ConsoleAppFacts
{
    public const string PROGRAM_CS_RELATIVE_FILE_PATH = @"Program.cs";

    public static string GetProgramCsContents(string projectName) => @$"// See https://aka.ms/new-console-template for more information
Console.WriteLine(""Hello, World!"");
";
}
