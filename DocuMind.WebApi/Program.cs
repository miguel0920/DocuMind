using DocuMind.Application; // <- Asegúrate de importar tus extensiones
using DocuMind.Infrastructure;
using Hangfire;
using Hangfire.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));

builder.Services.AddHangfireServer();

// 1. Agrega los controladores nativos (si la plantilla los creó)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// === AQUÍ CONECTAMOS NUESTRAS CAPAS DE CLEAN ARCHITECTURE ===
// 2. Registramos la lógica de aplicación (MediatR, Splitter, etc.)
builder.Services.AddApplicationServices();

// 3. Registramos la infraestructura (PostgreSQL, Repositorios, Fábrica de IA)
// Le pasamos el builder.Configuration para que pueda leer las API Keys y ConnectionStrings
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    // Si tienes Swagger o herramientas de desarrollo, van aquí
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseHangfireDashboard();

app.Run();