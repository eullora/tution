using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Zeeble.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<WebApiDBContext>(x => x.UseSqlite(connectionString));
builder.Services.AddScoped<IStorageService, StorageService>();

builder.Services.AddSession();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.SlidingExpiration = true;
            options.AccessDeniedPath = "/Error/NoAccess";
        });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-admin.json")),
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

else
{
    app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
#if DEBUG
app.UseHttpsRedirection();
#endif

app.UseStaticFiles();
app.UseRouting();

app.UseStatusCodePages(context =>
{
    var response = context.HttpContext.Response;

    if (response.StatusCode == 404)
    {
        response.Redirect("/Error/NoPage");
    }

    return Task.CompletedTask;
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("x-content-type-options", "nosniff");
    context.Response.Headers.Append("x-frame-options", "DENY");
    context.Response.Headers.Append("x-xss-protection", "1; mode=block");
    context.Response.Headers.Append("referrer-policy", "same-origin");
    await next();
});

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
