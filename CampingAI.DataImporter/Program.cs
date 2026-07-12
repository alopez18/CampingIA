using CampingAI.DataImporter;
using CampingAI.DataImporter.Configuration;
using CampingAI.DataImporter.Clients;
using CampingAI.DataImporter.Clients.Interfaces;
using CampingAI.DataImporter.Importers;
using CampingAI.DataImporter.Importers.Interfaces;
using CampingAI.DataImporter.Mappers;
using CampingAI.DataImporter.Services;
using CampingAI.DataImporter.Services.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
var config  = builder.Configuration;

// Configuración tipada
builder.Services.Configure<AppSettings>(config.GetSection(AppSettings.SECTION));

// HttpClient para Overpass API
builder.Services.AddHttpClient<IOverpassClient, OverpassClient>(client => {
    var baseUrl = config[$"{AppSettings.SECTION}:Overpass:BaseUrl"]
                  ?? "https://overpass-api.de/api/interpreter";
    var timeout = config.GetValue($"{AppSettings.SECTION}:Overpass:TimeoutSeconds", 120);
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout     = TimeSpan.FromSeconds(timeout);
});

// Servicios del importador
builder.Services.AddSingleton<CampingImportMapper>();
builder.Services.AddScoped<ICampingImportService, CampingImportService>();
builder.Services.AddScoped<ICampingsImporter,     CampingsImporter>();

// Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
