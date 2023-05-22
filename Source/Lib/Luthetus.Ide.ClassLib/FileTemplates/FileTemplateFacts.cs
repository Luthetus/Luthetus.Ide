using System.Collections.Immutable;
using System.Text;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.FileConstants;

namespace Luthetus.Ide.ClassLib.FileTemplates;

public static class FileTemplateFacts
{
    public static readonly IFileTemplate CSharpClass = new FileTemplate(
        "C# Class",
        "bstudio-c-sharp-class",
        FileTemplateKind.CSharp,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.C_SHARP_CLASS),
        _ => ImmutableArray<IFileTemplate>.Empty,
        true,
        CSharpClassCreateFileFunc);

    public static readonly IFileTemplate RazorMarkup = new FileTemplate(
        "Razor markup",
        "bstudio-razor-markup-class",
        FileTemplateKind.Razor,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP),
        _ => new[] { RazorCodebehind }.ToImmutableArray(),
        true,
        RazorMarkupCreateFileFunc);

    public static readonly IFileTemplate RazorCodebehind = new FileTemplate(
        "Razor codebehind",
        "bstudio-razor-codebehind-class",
        FileTemplateKind.Razor,
        filename => filename
            .EndsWith('.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND),
        _ => ImmutableArray<IFileTemplate>.Empty,
        true,
        RazorCodebehindCreateFileFunc);

    /// <summary>
    /// namespace Luthetus.Ide.ClassLib.FileTemplates;
    ///
    /// public class Asdf
    /// {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult CSharpClassCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"public class {fileNameNoExtension}{Environment.NewLine}");

            templateBuilder.Append(
                $"{{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);

        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension +
                                                 '.' +
                                                 ExtensionNoPeriodFacts.C_SHARP_CLASS;

        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }

    /// <summary>
    /// <h3>Asdf</h3>
    /// 
    /// @code {
    ///     
    /// }
    /// 
    /// </summary>
    private static FileTemplateResult RazorMarkupCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"<h3>{fileNameNoExtension}</h3>{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"@code {{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension);

        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension +
                                                 '.' +
                                                 ExtensionNoPeriodFacts.RAZOR_MARKUP;

        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }

    /// <summary>
    /// using Microsoft.AspNetCore.Components;
    /// 
    /// namespace Luthetus.Ide.RazorLib.Menu;
    /// 
    /// public partial class Asdf : ComponentBase
    /// {
    ///     
    /// }
    /// </summary>
    private static FileTemplateResult RazorCodebehindCreateFileFunc(
        FileTemplateParameter fileTemplateParameter)
    {
        string GetContent(string fileNameNoExtension, string namespaceString)
        {
            var templateBuilder = new StringBuilder();

            templateBuilder.Append(
                $"using Microsoft.AspNetCore.Components;{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"namespace {namespaceString};{Environment.NewLine}");

            templateBuilder.Append(
                Environment.NewLine);

            templateBuilder.Append(
                $"public partial class" +
                $" {fileNameNoExtension.Replace('.' + ExtensionNoPeriodFacts.RAZOR_MARKUP, string.Empty)}" +
                $" : ComponentBase{Environment.NewLine}");

            templateBuilder.Append(
                $"{{{Environment.NewLine}");

            templateBuilder.Append(
                $"\t{Environment.NewLine}");

            templateBuilder.Append(
                $"}}{Environment.NewLine}");

            return templateBuilder.ToString();
        }

        var emptyFileAbsoluteFilePathString = fileTemplateParameter
                                                  .ParentDirectory.AbsoluteFilePath
                                                  .GetAbsoluteFilePathString() +
                                              fileTemplateParameter.Filename;

        // Create AbsoluteFilePath as to leverage it for
        // knowing the file extension and other details
        var emptyFileAbsoluteFilePath = new AbsoluteFilePath(
            emptyFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileContent = GetContent(
            emptyFileAbsoluteFilePath.FileNameNoExtension,
            fileTemplateParameter.ParentDirectory.Namespace);

        var templatedFileFileAbsoluteFilePathString = fileTemplateParameter
                                                     .ParentDirectory.AbsoluteFilePath
                                                     .GetAbsoluteFilePathString() +
                                                 emptyFileAbsoluteFilePath.FileNameNoExtension;

        if (templatedFileFileAbsoluteFilePathString.EndsWith(
                '.' + ExtensionNoPeriodFacts.RAZOR_MARKUP))
        {
            templatedFileFileAbsoluteFilePathString +=
                '.' + ExtensionNoPeriodFacts.C_SHARP_CLASS;
        }
        else
        {
            templatedFileFileAbsoluteFilePathString +=
                '.' + ExtensionNoPeriodFacts.RAZOR_CODEBEHIND;
        }

        var templatedFileAbsoluteFilePath = new AbsoluteFilePath(
            templatedFileFileAbsoluteFilePathString,
            false,
            fileTemplateParameter.EnvironmentProvider);

        var templatedFileNamespacePath = new NamespacePath(
            fileTemplateParameter.ParentDirectory.Namespace,
            templatedFileAbsoluteFilePath);

        return new FileTemplateResult(templatedFileNamespacePath, templatedFileContent);
    }
}