using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Tutorials.RazorLib;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var hostingInformation = new LuthetusHostingInformation(
    LuthetusHostingKind.ServerSide,
    LuthetusPurposeKind.TextEditor,
    new BackgroundTaskService());
    
builder.Services.AddLuthetusTutorialsRazorLibServices(hostingInformation);

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
