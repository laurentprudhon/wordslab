using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using wordslab.web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// --- Use Entity Framework Core with Postgresql database
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContextFactory<WordslabContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("SchoolContext")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!IsDevelopment(app.Environment))
{
    app.UseExceptionHandler("/Error");
}

// --- Use Entity Framework Core with Postgresql database
if (IsDevelopment(app.Environment))
{
    app.UseDeveloperExceptionPage();
    await CreateDbIfNotExists(app);
}
// ---

app.UseStaticFiles();

app.UseRouting();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

// --- Use Entity Framework Core with Postgresql database
static async Task CreateDbIfNotExists(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var dbFactory = services.GetRequiredService<IDbContextFactory<WordslabContext>>();
            using var context = dbFactory.CreateDbContext();
            var neededToCreateDatabase = context.Database.EnsureCreated();
            await WordslabDbInitializer.InitializeAsync(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB.");
        }
    }
}
// ---

// Support for Kubernetes deployment
static bool IsDevelopment(IHostEnvironment hostEnvironment)
{
    return  hostEnvironment.IsDevelopment() || hostEnvironment.IsEnvironment("KubernetesDevelopment");
}