namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorServerEmptyCase;

public static partial class BlazorServerEmptyFacts
{
    public static string GetCsprojContents(string projectName) => @$"<Project Sdk=""Microsoft.NET.Sdk.Web"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
";
}
