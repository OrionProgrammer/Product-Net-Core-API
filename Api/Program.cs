using Api.Business_Rules;
using Api.Helpers;
using API.Helpers;
using API.Middleware;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repository.Helpers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var services = builder.Services;


services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();

services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", info: new OpenApiInfo { Title = "Product App API", Version = "v1" });
    option.OperationFilter<HeaderFilter>();
});

//automapper service
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
//database service
services.AddDbContext<DataContext>();


string corsName = "CorsName";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsName, policyBuilder => policyBuilder
        .WithOrigins("http://localhost", "https://localhost")
        .AllowAnyMethod()
        .AllowAnyHeader());
});


var appSettingsSection = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

// configure jwt authentication
var key = Encoding.ASCII.GetBytes(appSettingsSection.Secret);
services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var userId = int.Parse(context.Principal.Identity.Name);
            var user = userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                // return unauthorized if user no longer exists
                context.Fail("Unauthorized");
            }
            return Task.CompletedTask;
        }
    };
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

//dependencies
services.AddSingleton<IProductCodeSingleton, ProductCodeSingleton>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICategoryRepository, CategoryRepository>();
services.AddScoped<IProductRepository, ProductRepository>();
services.AddScoped<IUserCategoryRepository, UserCategoryRepository>();
services.AddSingleton<ProductCodeSingleton>();
services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //mny swagger is not working 100%, don't have enough time to correct it

    var swaggerConfig = new SwaggerConfig();
    builder.Configuration.GetSection(nameof(SwaggerConfig)).Bind(swaggerConfig);

    app.UseSwagger(option => { option.RouteTemplate = swaggerConfig.JsonRoute; });
    app.UseSwaggerUI(option => { option.SwaggerEndpoint(swaggerConfig.UIEndpoint, swaggerConfig.Description); });

}

// global cors policy
app.UseCors(x => x
    .SetIsOriginAllowed(origin => true)
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

// global error handler
app.UseMiddleware<ExceptionHandler>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Make the implicit Program class public so test projects can access it
public partial class Program { }