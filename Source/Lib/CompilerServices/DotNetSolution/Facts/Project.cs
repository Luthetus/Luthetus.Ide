namespace Luthetus.CompilerServices.DotNetSolution.Facts;

public partial class LexSolutionFacts
{
    public class Project
    {
        public const string PROJECT_DEFINITION_START_TOKEN = @"Project";
        public const string GUID_START_TOKEN = @"{";
        public const string GUID_END_TOKEN = @"}";
        public const string VALUE_START_TOKEN = @"""";
        public const string VALUE_END_TOKEN = @"""";
        public const string PROJECT_DEFINITION_END_TOKEN = @"EndProject";
    }
}
