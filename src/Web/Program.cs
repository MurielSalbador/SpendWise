using System.Reflection;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SpendWise.Core.Interfaces;
using SpendWise.Infrastructure.Repositories;
using SpendWise.Web.Services;
using Core.Services;
using SpendWise.Web.Middleware;
using SpendWise.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient("ExchangeRateClient", client =>
{
    client.BaseAddress = new Uri("https://dolarapi.com/v1/");
});
builder.Services.AddScoped<ExchangeRateService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomAuthenticationService, CustomAuthenticationService>();

builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<TransactionService>();

builder.Services.AddScoped<IUserRepository, UserRepository>(); // registra implementación
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<NoteService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();


builder.Services.Configure<CustomAuthenticationService.AutenticacionServiceOptions>(
    builder.Configuration.GetSection(CustomAuthenticationService.AutenticacionServiceOptions.SectionName)
);

builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();

#region MySQL config
var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("No se encontró la cadena de conexión de MySQL.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36))));
#endregion



#region Swagger custom token config
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("SpendWiseApiBearerAuth", new OpenApiSecurityScheme() // Esto permitw usar swagger con el token.
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Acá pegar el token generado al loguearse."
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "SpendWiseApiBearerAuth" } //Tiene que coincidir con el id seteado arriba en la definición
                }, new List<string>() }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    setupAction.IncludeXmlComments(xmlPath);

});

var secretKey = builder.Configuration["AutenticationService:SecretForKey"];
Console.WriteLine($"Clave JWT leída: {secretKey ?? "NULL"}");
if(string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("La clave secreta JWT no está configurada");
}
builder.Services.AddAuthentication("Bearer") //"Bearer" es el tipo de auntenticación que tenemos que elegir después en PostMan para pasarle el token
    .AddJwtBearer(options => //Acá definimos la configuración de la autenticación. le decimos qué cosas queremos comprobar. La fecha de expiración se valida por defecto.
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AutenticationService:Issuer"],
            ValidAudience = builder.Configuration["AutenticationService:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey))
        };
    }
);
# endregion


/* App */
var app = builder.Build();

// Swagger para custom token
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();