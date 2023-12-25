using Xunit;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// <see cref="ConstraintNode"/>
/// </summary>
public class ConstraintNodeTests
{
    /// <summary>
    /// <see cref="ConstraintNode(System.Collections.Immutable.ImmutableArray{RazorLib.CompilerServices.Syntax.ISyntaxToken})"/>
    /// </summary>
    [Fact]
	public void Constructor()
    {
        var constraintText = @"where T : notnull";
        var sourceText = $@"public abstract class TreeViewWithType<T> : TreeViewNoType {constraintText}
{{
}}";

        throw new NotImplementedException(_ = sourceText);
    }

    /// <summary>
    /// <see cref="ConstraintNode.InnerTokens"/>
    /// </summary>
    [Fact]
	public void InnerTokens()
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="ConstraintNode.ChildBag"/>
    /// </summary>
    [Fact]
	public void ChildBag()
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="ConstraintNode.IsFabricated"/>
    /// </summary>
    [Fact]
	public void IsFabricated()
	{
		throw new NotImplementedException();
	}

    /// <summary>
    /// <see cref="ConstraintNode.SyntaxKind"/>
    /// </summary>
    [Fact]
	public void SyntaxKind()
	{
		throw new NotImplementedException();
	}
}