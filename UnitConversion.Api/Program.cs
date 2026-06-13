using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using UnitConversion.Api.ApiServices.Conversion;
using UnitConversion.Domain.Core.Repositories.Conversion;
using UnitConversion.Domain.Core.Services.Conversion;
using UnitConversion.Domain.Infrastructure.Repositories.Conversion;
using UnitConversion.Domain.Infrastructure.Services.Conversion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Unit Conversion API",
        Version = "v1",
        Description = "Converts numeric values between supported units of measurement."
    });
});

builder.Services.AddHealthChecks();

builder.Services.AddScoped<IUnitConversionApiService, UnitConversionApiService>();
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();
builder.Services.AddSingleton<IUnitCatalogRepository, InMemoryUnitCatalogRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Unit Conversion API v1"));
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
