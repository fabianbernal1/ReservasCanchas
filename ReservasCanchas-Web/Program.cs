using Microsoft.EntityFrameworkCore;
using ReservasCanchas_Web.Components;
using ReservasCanchas_Web.Data;
using ReservasCanchas_Web.Servicios.Implementaciones;
using ReservasCanchas_Web.Servicios.Interfaces;
using ReservasCanchas_Web.Repositorio;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
