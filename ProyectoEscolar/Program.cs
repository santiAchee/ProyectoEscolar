using Microsoft.EntityFrameworkCore;
using ProyectoEscolar.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Lee directamente la cadena de conexión del appsettings.json (que ahora apunta a db-escolar)
var connectionString = builder.Configuration.GetConnectionString("MYSQL_CONNECTION_STRING_EF");

builder.Services.AddDbContext<SistemaEscolarContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0))));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
    options.AddPolicy("PermitirTodo", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// Creación automática de la base de datos y semilla
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SistemaEscolarContext>();
    db.Database.EnsureCreated();

    if (!db.TiposDocumentos.Any())
    {
        db.TiposDocumentos.AddRange(
            new ProyectoEscolar.Models.TiposDocumento { Nombre = "Fotocopia de DNI", Obligatorio = true, Activo = true },
            new ProyectoEscolar.Models.TiposDocumento { Nombre = "Libreta de Vacunas", Obligatorio = true, Activo = true },
            new ProyectoEscolar.Models.TiposDocumento { Nombre = "Certificado de Aptitud Física", Obligatorio = true, Activo = true },
            new ProyectoEscolar.Models.TiposDocumento { Nombre = "Pase de Escuela Anterior", Obligatorio = true, Activo = true }
        );

        db.Tutores.Add(new ProyectoEscolar.Models.Tutore { Nombre = "Admin", Apellido = "Sistema", Dni = "00000000", Tipo = "Tutor Legal", Activo = true });

        db.SaveChanges();
    }
}

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("PermitirTodo");
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/login.html"));

app.Run();