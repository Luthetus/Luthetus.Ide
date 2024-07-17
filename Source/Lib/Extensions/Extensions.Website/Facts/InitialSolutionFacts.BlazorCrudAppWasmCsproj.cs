namespace Luthetus.Ide.Wasm.Facts;

public partial class InitialSolutionFacts
{
    public const string BLAZOR_CRUD_APP_WASM_CSPROJ_ABSOLUTE_FILE_PATH = @"/BlazorCrudApp/BlazorCrudApp.Wasm/BlazorCrudApp.Wasm.csproj";
    public const string BLAZOR_CRUD_APP_WASM_CSPROJ_CONTENTS = @"<Project Sdk=""Microsoft.NET.Sdk.BlazorWebAssembly"">

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
