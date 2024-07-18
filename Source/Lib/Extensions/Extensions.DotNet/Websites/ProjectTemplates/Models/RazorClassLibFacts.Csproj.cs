namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public partial class RazorClassLibFacts
{
    public static string GetCsprojContents(string projectName) => @$"<Project Sdk=""Microsoft.NET.Sdk.Razor"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>


  <ItemGroup>
    <SupportedPlatform Include=""browser"" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.Web"" Version=""6.0.21"" />
  </ItemGroup>

</Project>
";
}
