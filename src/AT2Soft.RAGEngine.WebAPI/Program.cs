using AT2Soft.RAGEngine.Application;
using AT2Soft.RAGEngine.Application.Extensions;
using AT2Soft.RAGEngine.Infrastructure.Persistence;
using AT2Soft.RAGEngine.Infrastructure.SQLServer;
using AT2Soft.RAGEngine.WebAPI;
using AT2Soft.RAGEngine.Infrastructure;
using AT2Soft.RAGEngine.WebAPI.BackgroundServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Leer cadena de conexión
var relationalCnt = builder.Configuration.MustHaveConnectionString("RelationalDb");
var vectorialCnt = builder.Configuration.MustHaveConnectionString("VectorialDb");

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();

// Agrega Servico de DB Vectorial
builder.Services.AddRAGSqlServer(relationalCnt);

// Agrega Servico de DB Relacional
builder.Services.AddPersistenceQdrant(builder.Configuration);

builder.Services.AddRAGOllama(builder.Configuration);

builder.Services.AddRAGApplication();

builder.Services.AddRAGJwtAuthentication(builder.Configuration);

builder.Services.AddRAGTextExtractors();
builder.Services.AddRAGTextChunker();

builder.Services.AddRAGQueue(); 
builder.Services.AddHostedService<RagIngestWorker>();

var app = builder.Build();

// Middleware de logging de requests de Serilog (útil y muy barato)
app.UseSerilogRequestLogging(options =>
{
    // Ejemplo: añade duración, método, ruta y status code
    options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress?.ToString() ?? string.Empty);
        diagCtx.Set("UserAgent", httpCtx.Request.Headers.UserAgent.ToString() ?? string.Empty);
    };
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

try
{
    Log.Information("Arrancando Web API");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}