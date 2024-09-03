using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Installations.Models;

namespace Luthetus.TextEditor.Tests.Adhoc;

public class PartitionTests
{
	/// <summary>
	/// When a deletion of text spans multiple partitions it seems to sometimes break the line endings.
	///
	/// Why do I name things 'Aaa' on occassion?
	///
	/// "What Do You Do When You Can't Do Anything"
	/// https://youtu.be/JRqB2pm33IM?si=UhQxGhT8HcETN4xa
	///
	/// By using the name 'Aaa' consistently in this situations, I can easily find these naming scenarios
	/// and fix them when there is a better mood.
	/// </summary>
	[Fact]
	public void Aaa()
	{
		var test = TestInitialize();
		
		var modelList = test.TextEditorService.TextEditorStateWrap.Value.ModelList;
		Assert.Equal(0, modelList.Count);
		
		throw new NotImplementedException("In progress of writing this test");
	}
	
	public class TestContext
	{
		public TestContext(IServiceProvider serviceProvider)
		{
			ServiceProvider = serviceProvider;
			
			TextEditorService = ServiceProvider.GetRequiredService<ITextEditorService>();
		}
	
		public ITextEditorService TextEditorService { get; }
		public IServiceProvider ServiceProvider { get; }
	}
	
	public TestContext TestInitialize()
	{
		var services = new ServiceCollection();
		
		var hostingInformation = new LuthetusHostingInformation(
			LuthetusHostingKind.UnitTestingAsync,
	        LuthetusPurposeKind.TextEditor,
	        new BackgroundTaskService());
		
		services
			.AddLuthetusTextEditor(hostingInformation)
			.AddScoped<IJSRuntime, DoNothingJsRuntime>();
            
		// If one doesn't use a LuthetusPurposeKind of Photino,
		// then this code block is redundant, because the ServieCollectionExtensions.cs
		// for Luthetus.Common will register InMemory for Environment and FileSystem providers.
		//
		// But, because of the importance that under no circumstance one's actual FileSystem is used during
		// unit testing, this redundancy is here.
		{
			services.AddScoped<IEnvironmentProvider, InMemoryEnvironmentProvider>();
			services.AddScoped<IFileSystemProvider, InMemoryFileSystemProvider>();
		}
		
		// At the moment one has to manually set the following services to their InMemory implementation.
		{
			services.AddScoped<IClipboardService, InMemoryClipboardService>();
			services.AddScoped<IStorageService, InMemoryStorageService>();
		}
		
		services.AddFluxor(options => options.ScanAssemblies(
			typeof(LuthetusCommonConfig).Assembly,
			typeof(LuthetusTextEditorConfig).Assembly));
		
		var testContext = new TestContext(services.BuildServiceProvider());
		
		// Check various services that should be InMemory implementations.
		{	
			{
				var fileSystemProvider = testContext.ServiceProvider.GetRequiredService<IFileSystemProvider>();
				if (fileSystemProvider is not InMemoryFileSystemProvider)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IFileSystemProvider)} must be " +
						$"{nameof(InMemoryFileSystemProvider)}, " +
						$"but was {fileSystemProvider.GetType().Name}");
			}
			
			{
				var environmentProvider = testContext.ServiceProvider.GetRequiredService<IEnvironmentProvider>();
				if (environmentProvider is not InMemoryEnvironmentProvider)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IEnvironmentProvider)} must be " +
						$"{nameof(InMemoryEnvironmentProvider)}, " +
						$"but was {environmentProvider.GetType().Name}");
			}
	
			{
				var clipboardService = testContext.ServiceProvider.GetRequiredService<IClipboardService>();
				if (clipboardService is not InMemoryClipboardService)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IClipboardService)} must be " +
						$"{nameof(InMemoryClipboardService)}, " +
						$"but was {clipboardService.GetType().Name}");
			}
	
			{
				var storageService = testContext.ServiceProvider.GetRequiredService<IStorageService>();
				if (storageService is not InMemoryStorageService)
					throw new LuthetusUnitTestException(
						$"A UnitTest's {nameof(IStorageService)} must be " +
						$"{nameof(InMemoryStorageService)}, " +
						$"but was {storageService.GetType().Name}");
			}
		}
	
		return testContext;
	}
}
