using Apolon.Core.ORM;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    using (var scope = app.Services.CreateScope())
    {
        var dbService = scope.ServiceProvider.GetRequiredService<DatabaseService>();

        Console.WriteLine("\n--- Attempting to create database schema... ---");
        dbService.EnsureSchemaCreatedAsync(
            typeof(Apolon.Core.Models.Patient),
            typeof(Apolon.Core.Models.Checkup),
            typeof(Apolon.Core.Models.Medication),
            typeof(Apolon.Core.Models.Prescription)
        ).GetAwaiter().GetResult();
    }
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
