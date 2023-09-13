namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplatesCase.XUnitCase;

public partial class XUnitFacts
{
    public const string UNIT_TEST_1_CS_RELATIVE_FILE_PATH = @"Program.cs";

    public static string GetUnitTest1CsContents(string projectName) => @$"namespace {projectName}
{{
    public class UnitTest1
    {{
        [Fact]
        public void Test1()
        {{

        }}
    }}
}}";
}
