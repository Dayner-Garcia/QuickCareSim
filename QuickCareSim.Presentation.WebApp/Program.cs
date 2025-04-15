using Microsoft.AspNetCore.Identity;
using QuickCareSim.Application;
using QuickCareSim.Application.Settings;
using QuickCareSim.Domain.Enums;
using QuickCareSim.Infrastructure.Identity;
using QuickCareSim.Infrastructure.Identity.Entities;
using QuickCareSim.Infrastructure.Persistance;
using QuickCareSim.Infrastructure.Shared;

var builder = WebApplication.CreateBuilder(args);

// Conf and services
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddContextInfraestructure(builder.Configuration);
builder.Services.AddServicesForWebApp();
builder.Services.AddIdentityService();
builder.Services.AddSharedService();

// SMTP settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole(Roles.ADMIN.ToString()));
    options.AddPolicy("Client", policy => policy.RequireRole(Roles.CLIENT.ToString()));
    options.AddPolicy("Doctor", policy => policy.RequireRole(Roles.DOCTOR.ToString()));
    options.AddPolicy("Patient", policy => policy.RequireRole(Roles.PATIENT.ToString()));
    options.AddPolicy("Assistant", policy => policy.RequireRole(Roles.ASISTANT.ToString()));
    options.AddPolicy("Secretary", policy => policy.RequireRole(Roles.SECRETARY.ToString()));
});

// Controladores y vistas
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Ejecutar Seeders
await app.Services.RunIdentitySeeds();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();