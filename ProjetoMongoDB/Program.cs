using Microsoft.AspNetCore.Identity;
using ProjetoMongoDB.Models;
using ProjetoMongoDB.Services;
using ProjetoMongoDB.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexão com o MongoDB
ContextMongoDb.ConnectionString = builder.Configuration.GetSection("MongoConnection:ConnectionStrings").Value;
ContextMongoDb.DatabaseName = builder.Configuration.GetSection("MongoConnection:Database").Value;
ContextMongoDb.Isssl = Convert.ToBoolean(builder.Configuration.GetSection("MongoConnection:IsSsl").Value);

// Configuração do Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
    (ContextMongoDb.ConnectionString, ContextMongoDb.DatabaseName)
    .AddDefaultTokenProviders(); // Usado para gerar os tokens

// Configuração do Envio de E-mail
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailService"));
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Autenticação de usuário do Identity
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
