using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Website.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();

var hostingInformation = new LuthetusHostingInformation(
    LuthetusHostingKind.ServerSide,
    new BackgroundTaskService());

services.AddLuthetusIdeRazorLibServices(hostingInformation);

services.AddFluxor(options => options.ScanAssemblies(
	typeof(LuthetusCommonConfig).Assembly,
	typeof(LuthetusTextEditorConfig).Assembly,
	typeof(LuthetusIdeConfig).Assembly));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
