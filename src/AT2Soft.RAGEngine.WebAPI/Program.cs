using AT2Soft.RAGEngine.Application;
using AT2Soft.RAGEngine.Application.Extensions;
using AT2Soft.RAGEngine.Infrastructure.Persistence;
using AT2Soft.RAGEngine.Infrastructure.SQLServer;
using AT2Soft.RAGEngine.WebAPI;
using AT2Soft.RAGEngine.Infrastructure;
using AT2Soft.RAGEngine.WebAPI.BackgroundServices;
using AT2Soft.RAGEngine.WebAPI.HealthChecks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;

var builder = WebApplication.CreateBuilder(args);

// Leer cadena de conexión
var relationalCnt = builder.Configuration.MustHaveConnectionString("RelationalDb");
var vectorialCnt = builder.Configuration.MustHaveConnectionString("VectorialDb");

builder.Services.AddRAGHealthChecks(builder.Configuration);

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app. MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

app.Run();