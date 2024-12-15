using API.Middleware;
using API.SignalR;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(
    (setup) =>
    {
        setup.AddPolicy(
            "CorsPolicy",
            (options) =>
            {
                options
                    .WithOrigins("https://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }
        );
    }
);



builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var connString = builder.Configuration.GetConnectionString("Redis");
    if(connString == null ) throw new Exception("Cannot get redis connection string");
    var configuration = ConfigurationOptions.Parse(connString, true);

    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddSingleton<ICartService, CartService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
});

builder.Services.AddIdentityApiEndpoints<AppUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<StoreContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddSignalR();





var app = builder.Build();

app.Use(async (context, next) =>

{
    context.Response.Headers.Add("Content-Security-Policy", "script-src 'self' 'unsafe-inline' https://m.stripe.network;");
 
    Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
    Console.WriteLine($"Response: {context.Response.StatusCode}");
});
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();




// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();








app.MapControllers();
app.MapGroup("api").MapIdentityApi<AppUser>(); // api/login
app.MapHub<NotificationHub>("/hub/notifications");


try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context, userManager);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
