using Luthetus.CompilerServices.Lang.Css.Css.SyntaxEnums;
using Luthetus.CompilerServices.Lang.Css.Css.SyntaxObjects;

namespace Luthetus.CompilerServices.Lang.Css.Css.SyntaxActors;

public class CssSyntaxWalker
{
    public List<CssIdentifierSyntax> IdentifierSyntaxes { get; } = new();
    public List<CssCommentSyntax> CommentSyntaxes { get; } = new();
    public List<CssPropertyNameSyntax> PropertyNameSyntaxes { get; } = new();
    public List<CssPropertyValueSyntax> PropertyValueSyntaxes { get; } = new();

    public void Visit(ICssSyntax cssSyntax)
    {
        foreach (var child in cssSyntax.ChildCssSyntaxes)
            Visit(child);

        switch (cssSyntax.CssSyntaxKind)
        {
            case CssSyntaxKind.Identifier:
                VisitCssIdentifierSyntax((CssIdentifierSyntax)cssSyntax);
                break;
            case CssSyntaxKind.Comment:
                VisitCssCommentSyntax((CssCommentSyntax)cssSyntax);
                break;
            case CssSyntaxKind.PropertyName:
                VisitCssPropertyNameSyntax((CssPropertyNameSyntax)cssSyntax);
                break;
            case CssSyntaxKind.PropertyValue:
                VisitCssPropertyValueSyntax((CssPropertyValueSyntax)cssSyntax);
                break;
        }
    }

    public virtual void VisitCssIdentifierSyntax(CssIdentifierSyntax cssIdentifierSyntax)
    {
        IdentifierSyntaxes.Add(cssIdentifierSyntax);
    }

    public virtual void VisitCssCommentSyntax(CssCommentSyntax cssCommentSyntax)
    {
        CommentSyntaxes.Add(cssCommentSyntax);
    }

    public virtual void VisitCssPropertyNameSyntax(CssPropertyNameSyntax cssPropertyNameSyntax)
    {
        PropertyNameSyntaxes.Add(cssPropertyNameSyntax);
    }

    public virtual void VisitCssPropertyValueSyntax(CssPropertyValueSyntax cssPropertyValueSyntax)
    {
        PropertyValueSyntaxes.Add(cssPropertyValueSyntax);
    }
}