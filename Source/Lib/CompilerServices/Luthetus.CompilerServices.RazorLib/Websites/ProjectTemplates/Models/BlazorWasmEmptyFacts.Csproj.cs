namespace Luthetus.CompilerServices.RazorLib.Websites.ProjectTemplates.Models;

public static partial class BlazorWasmEmptyFacts
{
    public static string GetCsprojContents(string projectName) => @$"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly"" Version=""6.0.21"" />
    <PackageReference Include=""Microsoft.AspNetCore.Components.WebAssembly.DevServer"" Version=""6.0.21"" PrivateAssets=""all"" />
  </ItemGroup>

</Project>
";
}
