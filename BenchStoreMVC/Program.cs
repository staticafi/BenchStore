using BenchStoreBL;
using BenchStoreBL.Options;

using BenchStoreDAL;
using BenchStoreDAL.Data;

using Microsoft.Extensions.FileProviders;

using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .RegisterDALServices(builder.Configuration)
    .RegisterBLConfig(builder.Configuration)
    .RegisterBLServices();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<BenchStoreContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

}
// Configure the HTTP request pipeline.
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

string resultStorage;
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    IOptions<StorageOptions> options = services.GetRequiredService<IOptions<StorageOptions>>();
    resultStorage = options.Value.ResultStoragePath;
}

PhysicalFileProvider fileProvider = new PhysicalFileProvider(resultStorage);
string requestPath = "/Results";

// Enable displaying browser links.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});

app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = fileProvider,
    RequestPath = requestPath
});


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
