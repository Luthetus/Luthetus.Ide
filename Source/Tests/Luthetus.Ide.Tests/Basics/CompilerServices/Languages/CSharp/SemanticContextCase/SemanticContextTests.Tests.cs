namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.SemanticContextCase;

public partial class SemanticContextTests
{
    [Fact]
    public void SHOULD_CREATE_EMPTY_DOT_NET_SOLUTION_CONTEXT()
    {
        var solutionIdentifier = @"BlazorCrudApp";
        var resourceUri = new ResourceUri($"/{solutionIdentifier}.sln");

        var environmentProvider = new LocalEnvironmentProvider();

        var sourceText = $"{SLN_HEADER}{SLN_GLOBAL}".ReplaceLineEndings("\n");

        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
            resourceUri.Value,
            false,
            environmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            sourceText,
            dotNetSolutionNamespacePath,
            environmentProvider);

        var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
            DotNetSolutionKey.NewSolutionKey(),
            dotNetSolution);

        Assert.Empty(dotNetSolutionSemanticContext.DotNetProjectContextMap);
    }
    
    [Fact]
    public void SHOULD_CREATE_DOT_NET_SOLUTION_CONTEXT_WITH_ONE_PROJECT()
    {
        var solutionIdentifier = @"BlazorCrudApp";
        var resourceUri = new ResourceUri($"/{solutionIdentifier}.sln");

        var environmentProvider = new LocalEnvironmentProvider();

        var sourceText = $@"{SLN_HEADER}
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""BlazorWasmApp"", ""BlazorWasmApp\BlazorWasmApp.csproj"", ""{{A41C752D-A976-4337-8865-7EA232E0AE7E}}""
EndProject
{SLN_GLOBAL}".ReplaceLineEndings("\n");

        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
            resourceUri.Value,
            false,
            environmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            sourceText,
            dotNetSolutionNamespacePath,
            environmentProvider);

        var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
            DotNetSolutionKey.NewSolutionKey(),
            dotNetSolution);

        Assert.Single(dotNetSolutionSemanticContext.DotNetProjectContextMap);
    }
    
    [Fact]
    public void SHOULD_CREATE_DOT_NET_SOLUTION_CONTEXT_WITH_THREE_PROJECTS()
    {
        var solutionIdentifier = @"BlazorCrudApp";
        var resourceUri = new ResourceUri($"/{solutionIdentifier}.sln");

        var environmentProvider = new LocalEnvironmentProvider();

        var sourceText = $@"{SLN_HEADER}
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""BlazorCrudApp.Wasm"", ""BlazorCrudApp.Wasm\BlazorCrudApp.Wasm.csproj"", ""{{FAE29C14-2009-4A1B-AFBA-B5829B59EF59}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""BlazorCrudApp.ClassLib"", ""BlazorCrudApp.ClassLib\BlazorCrudApp.ClassLib.csproj"", ""{{1725E161-A8A1-49A1-A136-8426061403E9}}""
EndProject
Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""BlazorCrudApp.RazorLib"", ""BlazorCrudApp.RazorLib\BlazorCrudApp.RazorLib.csproj"", ""{{2069E609-2B98-43E1-930E-803C0C65725F}}""
EndProject
{SLN_GLOBAL}".ReplaceLineEndings("\n");

        var dotNetSolutionAbsoluteFilePath = new AbsoluteFilePath(
            resourceUri.Value,
            false,
            environmentProvider);

        var dotNetSolutionNamespacePath = new NamespacePath(
            string.Empty,
            dotNetSolutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            sourceText,
            dotNetSolutionNamespacePath,
            environmentProvider);

        var dotNetSolutionSemanticContext = new DotNetSolutionSemanticContext(
            DotNetSolutionKey.NewSolutionKey(),
            dotNetSolution);

        Assert.Equal(3, dotNetSolutionSemanticContext.DotNetProjectContextMap.Count);
    }
}
