using Redis.OM;
using RedisShop.Services;

string _className = "Program";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure redis
var connectionString = builder.Configuration.GetConnectionString("Redis");

if (string.IsNullOrEmpty(connectionString))
{
    RedisShop.Utils.Logger.Log(_className, LogType.ERROR, "Couldn't find connection string!");
    throw new Exception("Error in connection string!");
}

// Register Redis provider
builder.Services.AddSingleton(new RedisConnectionProvider(connectionString));

// Register services
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Redis provider
var provider = app.Services.GetRequiredService<RedisConnectionProvider>();

// Register indexes
await provider.Connection.CreateIndexAsync(typeof(RedisShop.Models.Product)); // Product

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
