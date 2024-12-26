using System.Collections.Immutable;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;

public static class WebsiteProjectTemplateFacts
{
	static WebsiteProjectTemplateFacts()
	{
		BlazorWasmEmptyProjectTemplate = new ProjectTemplate(
			"Blazor WebAssembly App Empty",
			"blazorwasm-empty",
			"[C#]",
			"Web/Blazor/WebAssembly/PWA/Empty");

		ConsoleAppProjectTemplate = new ProjectTemplate(
			"Console App",
			"console",
			"[C#],F#,VB",
			"Common/Console");

		WebsiteProjectTemplatesContainer = new[]
		{
			BlazorWasmEmptyProjectTemplate,
			ConsoleAppProjectTemplate,
		}.ToImmutableArray();
	}

	public static ProjectTemplate BlazorWasmEmptyProjectTemplate { get; }
	public static ProjectTemplate ConsoleAppProjectTemplate { get; }

	public static ImmutableArray<ProjectTemplate> WebsiteProjectTemplatesContainer { get; }

	public static async Task HandleNewCSharpProjectAsync(
		string projectTemplateShortName,
		string cSharpProjectAbsolutePathString,
		IFileSystemProvider fileSystemProvider,
		IEnvironmentProvider environmentProvider)
	{
		if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
			await HandleBlazorWasmEmptyProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
				.ConfigureAwait(false);
		else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
			await HandleConsoleAppProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
				.ConfigureAwait(false);
		else
			throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
	}

	private static async Task HandleBlazorWasmEmptyProjectTemplateAsync(
		string cSharpProjectAbsolutePathString,
		IFileSystemProvider fileSystemProvider,
		IEnvironmentProvider environmentProvider)
	{
		var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
		var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

		if (parentDirectoryOfProject is null)
			throw new NotImplementedException();

		var parentDirectoryOfProjectAbsolutePath = parentDirectoryOfProject;

		// AppCss
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.APP_CSS_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetAppCssContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// Csproj
		{
			await fileSystemProvider.File.WriteAllTextAsync(
					cSharpProjectAbsolutePathString,
					BlazorWasmEmptyFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// ImportsRazor
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// IndexHtml
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.INDEX_HTML_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetIndexHtmlContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// IndexRazor
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetIndexRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// LaunchSettingsJson
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// MainLayoutRazor
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// ProgramCs
		{
			var absolutePath = environmentProvider.JoinPaths(
				parentDirectoryOfProjectAbsolutePath.Value,
				BlazorWasmEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					BlazorWasmEmptyFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}
	}
	
	private static async Task HandleConsoleAppProjectTemplateAsync(
		string cSharpProjectAbsolutePathString,
		IFileSystemProvider fileSystemProvider,
		IEnvironmentProvider environmentProvider)
	{
		var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
		var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

		if (parentDirectoryOfProject is null)
			throw new NotImplementedException();

		var ancestorDirectory = parentDirectoryOfProject;

		// ProgramCs
		{
			var absolutePath = environmentProvider.JoinPaths(
				ancestorDirectory.Value,
				ConsoleAppFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

			await fileSystemProvider.File.WriteAllTextAsync(
					absolutePath,
					ConsoleAppFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}

		// Csproj
		{
			await fileSystemProvider.File.WriteAllTextAsync(
					cSharpProjectAbsolutePathString,
					ConsoleAppFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
				.ConfigureAwait(false);
		}
	}

	public static Guid GetProjectTypeGuid(string projectTemplateShortName)
	{
		// I'm not going to DRY up the string "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC" for now,
		// because I don't fully understand its purpose.

		if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
			return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
		else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
			return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
		else
			throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
	}


}
