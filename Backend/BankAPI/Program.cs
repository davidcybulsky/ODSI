using BankAPI.Data;
using BankAPI.Interfaces;
using BankAPI.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
IServiceCollection services = builder.Services;

//Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

//Controllers
services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
);

//Services
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IAuthService, AuthService>();
services.AddScoped<IDebitCardService, DebitCardService>();
services.AddScoped<IHttpContextService, HttpContextService>();
services.AddScoped<ITransferService, TransferService>();
services.AddScoped<IDocumentService, DocumentService>();
services.AddScoped<ISessionService, SessionService>();
services.AddScoped<SeedService>();

//DbContext
services.AddDbContext<ApiContext>(opt =>
{
    opt.UseNpgsql(configuration.GetConnectionString("Default"));
});

//DataProtection
services.AddDataProtection().PersistKeysToDbContext<ApiContext>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
using (ApiContext? db = services.BuildServiceProvider().GetService<ApiContext>())
{
    SeedService seedService = services.BuildServiceProvider().GetService<SeedService>();
    {
        db!.Database.Migrate();
        if (!db.Users.Any())
        {
            seedService!.Seed();
        }
    }
}
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

//Automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//HttpContext
services.AddHttpContextAccessor();

//Authentication
services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

//Authorization
services.AddAuthorization();

//Cors
#pragma warning disable CS8604 // Possible null reference argument.
services.AddCors(opt =>
    opt.AddDefaultPolicy(policy => policy.AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials()
                                        .WithOrigins(configuration["Cors:Web"]))
);
#pragma warning restore CS8604 // Possible null reference argument.

WebApplication app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors();

//app.UseHttpsRedirection();

app.UseCustomErrorHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();