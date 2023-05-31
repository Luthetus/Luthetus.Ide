namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp;

/// <summary>This class is used for testing the: lexer, parser, and evaluator logic. It contains two properties: a string resourceUri and a string content.</summary>
internal class TestResource
{
    public TestResource(
        string resourceUri,
        string content)
    {
        ResourceUri = resourceUri;
        Content = content;
    }

    public string ResourceUri { get; }
    public string Content { get; }
}
