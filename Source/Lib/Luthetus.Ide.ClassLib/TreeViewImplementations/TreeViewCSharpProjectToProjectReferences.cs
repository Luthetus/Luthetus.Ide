using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.DotNet.CSharp;
using Luthetus.TextEditor.RazorLib.Analysis.Html.SyntaxActors;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewCSharpProjectToProjectReferences : TreeViewWithType<CSharpProjectToProjectReferences>
{
    public TreeViewCSharpProjectToProjectReferences(
        CSharpProjectToProjectReferences cSharpProjectToProjectReferences,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectToProjectReferences,
                isExpandable,
                isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null ||
            obj is not TreeViewCSharpProjectToProjectReferences treeViewCSharpProjectToProjectReferences ||
            Item is null)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return Item?
            .GetHashCode()
               ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewCSharpProjectToProjectReferencesRendererType!,
            null);
    }

    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        var content = await FileSystemProvider.File.ReadAllTextAsync(
            Item.CSharpProjectNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString());

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            new(Item.CSharpProjectNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString()),
            content);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

        cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

        var projectReferences = cSharpProjectSyntaxWalker.TagSyntaxes
            .Where(ts => (ts.OpenTagNameSyntax?.Value ?? string.Empty) == "ProjectReference")
            .ToList();

        List<CSharpProjectToProjectReference> cSharpProjectToProjectReferences = new();

        foreach (var projectReference in projectReferences)
        {
            var attributeNameValueTuples = projectReference
                .AttributeSyntaxes
                .Select(x => (
                    x.AttributeNameSyntax.TextEditorTextSpan
                        .GetText()
                        .Trim(),
                    x.AttributeValueSyntax.TextEditorTextSpan
                        .GetText()
                        .Replace("\"", string.Empty)
                        .Replace("=", string.Empty)
                        .Trim()))
                .ToArray();

            var includeAttribute = attributeNameValueTuples
                .FirstOrDefault(x => x.Item1 == "Include");

            var referenceProjectAbsoluteFilePathString = AbsoluteFilePath
                .JoinAnAbsoluteFilePathAndRelativeFilePath(
                    Item.CSharpProjectNamespacePath.AbsoluteFilePath,
                    includeAttribute.Item2,
                    EnvironmentProvider);

            var referenceProjectAbsoluteFilePath = new AbsoluteFilePath(
                referenceProjectAbsoluteFilePathString,
                false,
                EnvironmentProvider);

            var cSharpProjectToProjectReference = new CSharpProjectToProjectReference(
                this.Item.CSharpProjectNamespacePath,
                referenceProjectAbsoluteFilePath);

            cSharpProjectToProjectReferences.Add(cSharpProjectToProjectReference);
        }

        var newChildren = cSharpProjectToProjectReferences
            .Select(x => (TreeViewNoType)new TreeViewCSharpProjectToProjectReference(
                x,
                LuthetusIdeComponentRenderers,
                FileSystemProvider,
                EnvironmentProvider,
                false,
                false)
            {
                TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
            })
            .ToList();

        for (int i = 0; i < newChildren.Count; i++)
        {
            var newChild = newChildren[i];

            newChild.IndexAmongSiblings = i;
            newChild.Parent = this;
            newChild.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
        }

        Children = newChildren;
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}