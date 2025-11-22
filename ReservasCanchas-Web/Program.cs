using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Microsoft.Extensions.Options;

using ReservasCanchas_Web.Components;
using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Repositorio;
using ReservasCanchas_Web.Servicios.Implementaciones;
using ReservasCanchas_Web.Servicios.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ReservasCanchas API", Version = "v1" });
});

// Configuración SMTP desde appsettings (usar SmtpOptions)
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();

// HttpClient para que los componentes consuman la API internamente
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5092");
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar DbContext usando la cadena DefaultConnection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<ICanchaRepository, CanchaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEstadoReservaRepository, EstadoReservaRepository>();
builder.Services.AddScoped<IMetodoPagoRepository, MetodoPagoRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();

// Servicios
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<ICanchaService, CanchaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEstadoReservaService, EstadoReservaService>();
builder.Services.AddScoped<IMetodoPagoService, MetodoPagoService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Web API, JSON y Swagger
builder.Services.AddControllers()
    .AddJsonOptions(opts => {
        // configura serialización si es necesario
    });

builder.Services.AddEndpointsApiExplorer();

// CORS (si en el futuro separas front en otra app)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithOrigins("https://localhost:5001", "http://localhost:5000", "http://localhost:5092");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReservasCanchas API v1"));
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseCors("AllowLocalhost");

app.UseAntiforgery();

app.MapControllers(); // Mapear APIs

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
