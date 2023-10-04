namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSectionNestedProjects
{
    public const string START_TOKEN_ORDER = @"preSolution";

    public const string FIRST_PROPERTY_NAME = @"{9061330A-ABDC-4999-9B51-E7B7E19D654C}";
    public const string FIRST_PROPERTY_VALUE = @"{8F15D57B-A808-4B20-8CA0-239D91F562BA}";

    public const string SECOND_PROPERTY_NAME = @"{5504FC02-FA1B-46DF-B6FA-25B363E50263}";
    public const string SECOND_PROPERTY_VALUE = @"{30DFD93C-D447-4D6F-9E34-2BC967366924}";

    public const string FULL =
        @"GlobalSection(NestedProjects) = preSolution
		{9061330A-ABDC-4999-9B51-E7B7E19D654C} = {8F15D57B-A808-4B20-8CA0-239D91F562BA}
		{5504FC02-FA1B-46DF-B6FA-25B363E50263} = {30DFD93C-D447-4D6F-9E34-2BC967366924}
	EndGlobalSection";
}
