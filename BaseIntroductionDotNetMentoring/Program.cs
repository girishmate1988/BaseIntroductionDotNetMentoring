using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BaseIntroductionDotNetMentoring.Data;
using BaseIntroductionDotNetMentoring.Helpers;
using BaseIntroductionDotNetMentoring.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ========== Configure Services (Startup.ConfigureServices equivalent) ==========

// 1. Configure Logging - Register ILogger<T> for dependency injection
builder.Services.AddLogging(configure =>
{
    configure.ClearProviders();
    configure.AddConsole();
    configure.AddDebug();
    configure.SetMinimumLevel(LogLevel.Information);
});

// 2. Add MVC Controllers and Views Support
builder.Services.AddControllersWithViews();

// 3. Register Entity Framework Core DbContext for Northwind Database
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Northwind"))
);

// 4. Bind and Register Configuration Settings
builder.Services.Configure<ProductSettings>(
    builder.Configuration.GetSection("ProductSettings")
);

// 5. Register Additional Services (Extensible for future services)
// Uncomment and use as needed:
// builder.Services.AddScoped<IProductService, ProductService>();
// builder.Services.AddScoped<ICategoryService, CategoryService>();
// builder.Services.AddScoped<ISupplierService, SupplierService>();

var app = builder.Build();

// ========== Configure HTTP Request Pipeline ==========

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Handle non-exception HTTP error responses (404, 400, etc.) with a user-friendly page
app.UseStatusCodePagesWithReExecute("/Home/Error");

// Log all unhandled exceptions with full details before the outer handler processes them
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
