using BackEndTest.Repository;
using BackEndTest.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Organizer.Context;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IBaseRepository, BaseRepository>();
builder.Services.AddScoped<IBankAccountRepository, BankAccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST")?? "localhost,8002";
var dbName = Environment.GetEnvironmentVariable("DB_NAME")?? "backendOrganizer";
var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD") ?? "password@12345#";
var connectionString = $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa;Password={dbPassword};TrustServerCertificate=true";

builder.Services.AddDbContext<AppDbContext>(options
    => options.UseSqlServer(connectionString));

//builder.Services.AddDbContext<AppDbContext>(options
//    => options.UseSqlServer(builder.Configuration.GetConnectionString("production")));

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "LogEntries",
            AutoCreateSqlTable = true
        })
    .CreateLogger();





builder.Services.AddAuthorization();


builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddApiEndpoints();

//builder.Services.AddAuthentication()
//    .AddBearerToken(IdentityConstants.BearerScheme);

//builder.Services.AddAuthorizationBuilder()
//    .AddPolicy("api", p =>
//    {
//        p.RequireAuthenticatedUser();
//        p.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
//    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGroup("api/auth")
    .MapIdentityApi<IdentityUser>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
