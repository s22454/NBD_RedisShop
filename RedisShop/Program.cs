using Microsoft.AspNetCore.Authentication.Cookies;
using Redis.OM;
using RedisShop.Auth;
using RedisShop.Services;

string _className = "Program";

var builder = WebApplication.CreateBuilder(args);

// Redis cache config
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis").Replace("redis://", "");
    options.InstanceName = "RedisShop_"; //todo move to config?
});

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
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<ITicketStore, RedisTicketStore>();

// Redis session managment config
builder.Services.AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<ITicketStore>((options, store) =>
    {
        options.SessionStore = store;

        options.ExpireTimeSpan = TimeSpan.FromSeconds(120);
        options.SlidingExpiration = false;
        options.LoginPath = "/Account/Login";
        options.Cookie.Name = "TokenThatDeservesAnA";
    });

// Activate authentication
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

var app = builder.Build();

// Redis provider
var provider = app.Services.GetRequiredService<RedisConnectionProvider>();

// Register indexes
//todo add error handling
await provider.Connection.CreateIndexAsync(typeof(RedisShop.Models.Product)); // Product
await provider.Connection.CreateIndexAsync(typeof(RedisShop.Models.User)); // User

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
