using CarRental.Repositories.DBContext;
using CarRental.Repositories.FollowVehicle;
using CarRental.Repositories.PostVehicle;
using CarRental.Repositories.RentVehicle;
using CarRental.Repositories.ReviewVehicle;
using CarRental.Repositories.User;
using CarRental.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbContext")));

builder.Services.AddControllersWithViews();
builder.Services
    .AddAuthentication("UserAuth")
    .AddCookie("UserAuth", options =>
    {
        options.LoginPath = "/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.AccessDeniedPath = "/";
        options.LogoutPath = "/auth/logout";
        options.Cookie.Name = "User";
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddScoped<IMailService, MailService>()
    .AddScoped<IUploadService, UploadService>()
    .AddScoped<IPostVehicleRepository, PostVehicleRepository>()
    .AddScoped<IRentVehicleRepository, RentVehicleRepository>()
    .AddScoped<IFollowVehicleRepository, FollowVehicleRepository>()
    .AddScoped<IReviewVehicleRepository, ReviewVehicleRepository>()
    .AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();
app.UseCookiePolicy();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    using (var appContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
    {
        try
        {
            appContext.Database.Migrate();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
app.Run();