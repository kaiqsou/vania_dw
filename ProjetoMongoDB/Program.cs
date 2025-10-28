using Microsoft.AspNetCore.Identity;
using ProjetoMongoDB.Models;
using ProjetoMongoDB.Services;
using ProjetoMongoDB.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjetoMongoDB.Seed;
using System;
using ProjetoMongoDB.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexao com o MongoDB
ContextMongoDb.ConnectionString = builder.Configuration.GetSection("MongoConnection:ConnectionStrings").Value;
ContextMongoDb.DatabaseName = builder.Configuration.GetSection("MongoConnection:Database").Value;
ContextMongoDb.Isssl = Convert.ToBoolean(builder.Configuration.GetSection("MongoConnection:IsSsl").Value);

// Configuracao do Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
    (ContextMongoDb.ConnectionString, ContextMongoDb.DatabaseName)
    .AddDefaultTokenProviders(); // Usado para gerar os tokens

// Configuracao do Envio de E-mail
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSingleton<EmailService>();

builder.Services.AddScoped<ContextMongoDb>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();

var app = builder.Build();

// Lógica da assinatura no evento
using (var scope = app.Services.CreateScope())
{
    // Obtém a instância Singleton do EmailService
    var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

    // Assina o evento! Agora, toda vez que o evento for disparado, o método HandleRegistroAsync será chamado.
    EventoNotifier.OnParticipanteRegistrado += emailService.HandleRegistroAsync;

    // Obtém a senha da nova seção AdminSettings no appsettings.json
    var defaultAdminPassword = app.Configuration["AdminSettings:DefaultPassword"] ?? throw new InvalidOperationException("AdminSettings:DefaultPassword não configurada.");

    // Chama o método de seed para criar a Role e o Usuário Admin
    await IdentitySeeder.SeedRolesAndAdminUser(scope.ServiceProvider, defaultAdminPassword);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Autentica��o de usu�rio do Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
