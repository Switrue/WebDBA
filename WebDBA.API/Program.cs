using WebDBA.API.Configurations;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

try
{
    services.AddControllers();

    // Connectiong services
    services.AddDependencies();
    services.AddOpenApi();
    services.AddSwaggerGen();
    services.AddDbContextConfiguration(configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        // Connectiong Swagger
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductsManagement API V1");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("Произошла ошибка: " + ex.Message);
    return;
}

