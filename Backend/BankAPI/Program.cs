using BankAPI.Data;
using BankAPI.Interfaces;
using BankAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

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

//DbContext
services.AddDbContext<ApiContext>(opt =>
{
    opt.UseNpgsql(configuration.GetConnectionString("Default"));
});

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
using (var db = services.BuildServiceProvider().GetService<ApiContext>())
{
    db!.Database.Migrate();
}
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

//Automapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

//HttpContext
services.AddHttpContextAccessor();

//Authentication
services.AddAuthentication()
    .AddCookie();

//Authorization
services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCustomErrorHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();