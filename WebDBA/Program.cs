using WebDBA.Configuration;
using WebDBA.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

try
{
    // Connectiong services
    services.AddControllersWithViews();
    services.AddApiServices(configuration);
    services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(JsonSettings.ConfigureJsonOptions);

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Workers/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseAuthorization();

    app.MapStaticAssets();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Workers}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Произошла ошибка: " + ex.ToString());
}
