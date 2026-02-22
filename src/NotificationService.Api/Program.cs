using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower)); // no enums in the api are used so far
    });

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
