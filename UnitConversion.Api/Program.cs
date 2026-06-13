using System.Text.Json.Serialization;
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
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("DefaultCorsPolicy");
app.MapControllers();

app.Run();
