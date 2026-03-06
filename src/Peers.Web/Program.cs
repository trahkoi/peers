using Peers.Training.Sessions;
using Peers.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

if (builder.Configuration.GetConnectionString("Sessions") is { } connectionString)
    builder.Services.AddSqliteSessions(connectionString);
else
    builder.Services.AddSessions();

builder.Services.Configure<AdminCredentials>(
    builder.Configuration.GetSection("AdminCredentials"));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/login";
        options.Cookie.IsEssential = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    });

var app = builder.Build();

if (app.Configuration.GetConnectionString("Sessions") is not null)
{
    app.Services.MigrateSessionsDatabase();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
