using EventCorp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Q_Manage.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("EventCorp") ?? throw new InvalidOperationException("Connection string 'EventCorp' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapGet("/api/events", async (ApplicationDbContext db) =>
{
    var hoy = DateTime.Today;

    var eventos = await db.Eventos
        .Include(e => e.Categoria)
        .Where(e => e.Fecha >= hoy)
        .OrderBy(e => e.Fecha)
        .Select(e => new
        {
            e.Id,
            e.Titulo,
            e.Descripcion,
            e.Fecha,
            e.Hora,
            e.DuracionMinutos,
            e.Ubicacion,
            e.CupoMaximo,
            Categoria = e.Categoria != null ? e.Categoria.Nombre : null
        })
        .ToListAsync();

    return Results.Ok(eventos);
});

app.MapGet("/api/events/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var evento = await db.Eventos
        .Include(e => e.Categoria)
        .FirstOrDefaultAsync(e => e.Id == id);

    if (evento == null)
        return Results.NotFound();

    return Results.Ok(new
    {
        evento.Id,
        evento.Titulo,
        evento.Descripcion,
        evento.Fecha,
        evento.Hora,
        evento.DuracionMinutos,
        evento.Ubicacion,
        evento.CupoMaximo,
        Categoria = evento.Categoria != null ? evento.Categoria.Nombre : null
    });
});


app.Run();