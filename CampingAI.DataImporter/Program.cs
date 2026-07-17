using CampingAI.DataImporter;
using CampingAI.DataImporter.Clients;
using CampingAI.DataImporter.Clients.Interfaces;
using CampingAI.DataImporter.Configuration;
using CampingAI.DataImporter.Importers;
using CampingAI.DataImporter.Importers.Interfaces;
using CampingAI.DataImporter.Mappers;
using CampingAI.DataImporter.Orchestration;
using CampingAI.DataImporter.Orchestration.Interfaces;
using CampingAI.DataImporter.Services;
using CampingAI.DataImporter.Services.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
var config = builder.Configuration;

// Configuración tipada
builder.Services.Configure<AppSettings>(config.GetSection(AppSettings.SECTION));

// HttpClient para Nominatim (geocodificación inversa — requiere User-Agent y máx. 1 req/s)
builder.Services.AddHttpClient<INominatimClient, NominatimClient>(client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("CampingAI-DataImporter/1.0 (+https://github.com/alopez18/CampingIA)");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

// HttpClient para Overpass API
builder.Services.AddHttpClient<IOverpassClient, OverpassClient>(client =>
{
    var baseUrl = config[$"{AppSettings.SECTION}:Overpass:BaseUrl"]
                  ?? "https://overpass-api.de/api/interpreter";
    var timeout = config.GetValue($"{AppSettings.SECTION}:Overpass:TimeoutSeconds", 120);
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(timeout);
    // Overpass rechaza (502/429) peticiones sin User-Agent identificable.
    client.DefaultRequestHeaders.UserAgent.ParseAdd("CampingAI-DataImporter/1.0 (+https://github.com/alopez18/CampingIA)");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

// Servicios del importador
builder.Services.AddSingleton<CampingImportMapper>();
builder.Services.AddScoped<ICampingImportService, CampingImportService>();
builder.Services.AddScoped<ICampingsImporter, CampingsImporter>();

// Servicios de migración T_CAMPINGS_IMPORT → T_CAMPINGS
builder.Services.AddSingleton<IProvinceGeoResolver, ProvinceGeoResolver>();
builder.Services.AddScoped<ICampingMigrationService, CampingMigrationService>();
builder.Services.AddScoped<IFacilitySeederService, FacilitySeederService>();
builder.Services.AddScoped<ICategorySeederService, CategorySeederService>();
builder.Services.AddScoped<ICampingMigrationImporter, CampingMigrationImporter>();

// Registro de fuentes de datos para la orquestación
builder.Services.AddScoped<IDataSourceImporter>(sp => sp.GetRequiredService<ICampingsImporter>());
builder.Services.AddScoped<IDataSourceImporter>(sp => sp.GetRequiredService<ICampingMigrationImporter>());
builder.Services.AddScoped<IImportOrchestrator, ImportOrchestrator>();

// Worker
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
