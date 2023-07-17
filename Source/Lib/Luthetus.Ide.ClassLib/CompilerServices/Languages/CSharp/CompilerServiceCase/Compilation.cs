namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.CompilerServiceCase;

/// <summary>
/// The result from having compiled the various resources.
/// </summary>
public record Compilation
{
    public Compilation(
        ParseTree parseTree,
        AbstractSyntaxTree abstractSyntaxTree)
    {
        ParseTree = parseTree;
        AbstractSyntaxTree = abstractSyntaxTree;
    }

    public ParseTree ParseTree { get; }
    public AbstractSyntaxTree AbstractSyntaxTree { get; }
}
