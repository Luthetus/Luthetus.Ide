using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Storages.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;

namespace Luthetus.BUnit.Tests;

public static class SharedInitializationForTests
{
	public static void Initialize(TestContext ctx)
	{
		var hostingInformation = new LuthetusHostingInformation(
			LuthetusHostingKind.UnitTestingAsync,
			new BackgroundTaskService());
		
		ctx.Services.AddLuthetusIdeRazorLibServices(hostingInformation);
		
		ctx.Services.AddFluxor(options => options.ScanAssemblies(
			typeof(LuthetusCommonConfig).Assembly,
			typeof(LuthetusTextEditorConfig).Assembly,
			typeof(LuthetusIdeConfig).Assembly));

		// Overwrite some services.
		//
		// This code block isn't preferable, but setting these
		// through the 'AddLuthetusIdeRazorLibServices(...)'
		// would be a bit extensive.
		//
		// The IFileSystemProvider, and IEnvironmentProvider however,
		// should not be in here, it is vital that they are reliably
		// NOT using the local filesystem.
		//
		// IClipboardService, and IStorageService on the other hand
		// will likely just throw a JavaScript interop exception
		// in the case that this hack doesn't work.
		{
			ctx.Services.AddScoped<IClipboardService, InMemoryClipboardService>();
			ctx.Services.AddScoped<IStorageService, InMemoryStorageService>();
		}

		var storeInitializer = ctx.RenderComponent<Fluxor.Blazor.Web.StoreInitializer>();
	}
}
