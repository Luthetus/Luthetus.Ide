using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.NewInterfaceCase;

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
