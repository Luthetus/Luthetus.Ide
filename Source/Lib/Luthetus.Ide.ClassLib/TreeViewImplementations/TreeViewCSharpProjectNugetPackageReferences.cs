using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.Nuget;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.Xml.Html.SyntaxActors;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewCSharpProjectNugetPackageReferences : TreeViewWithType<CSharpProjectNugetPackageReferences>
{
    public TreeViewCSharpProjectNugetPackageReferences(
        CSharpProjectNugetPackageReferences cSharpProjectNugetPackageReferences,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectNugetPackageReferences,
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
            obj is not TreeViewCSharpProjectNugetPackageReferences otherTreeView)
        {
            return false;
        }

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        return Item.CSharpProjectNamespacePath.AbsoluteFilePath
            .GetAbsoluteFilePathString()
            .GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewCSharpProjectNugetPackageReferencesRendererType!,
            null);
    }

    public override async Task LoadChildrenAsync()
    {
        var content = await FileSystemProvider.File.ReadAllTextAsync(
            Item.CSharpProjectNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString());

        var htmlSyntaxUnit = HtmlSyntaxTree.ParseText(
            new(Item.CSharpProjectNamespacePath.AbsoluteFilePath.GetAbsoluteFilePathString()),
            content);

        var syntaxNodeRoot = htmlSyntaxUnit.RootTagSyntax;

        var cSharpProjectSyntaxWalker = new CSharpProjectSyntaxWalker();

        cSharpProjectSyntaxWalker.Visit(syntaxNodeRoot);

        var packageReferences = cSharpProjectSyntaxWalker.TagNodes
            .Where(ts => (ts.OpenTagNameNode?.TextEditorTextSpan.GetText() ?? string.Empty) == "PackageReference")
            .ToList();

        List<LightWeightNugetPackageRecord> lightWeightNugetPackageRecords = new();

        foreach (var packageReference in packageReferences)
        {
            var attributeNameValueTuples = packageReference
                .AttributeNodes
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

            var versionAttribute = attributeNameValueTuples
                .FirstOrDefault(x => x.Item1 == "Version");

            var lightWeightNugetPackageRecord = new LightWeightNugetPackageRecord(
                includeAttribute.Item2,
                includeAttribute.Item2,
                versionAttribute.Item2);

            lightWeightNugetPackageRecords.Add(lightWeightNugetPackageRecord);
        }

        var cSharpProjectAbsoluteFilePathString = Item.CSharpProjectNamespacePath.AbsoluteFilePath
            .GetAbsoluteFilePathString();

        var newChildren = lightWeightNugetPackageRecords
            .Select(npr => (TreeViewNoType)new TreeViewCSharpProjectNugetPackageReference(
                new(cSharpProjectAbsoluteFilePathString, npr),
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