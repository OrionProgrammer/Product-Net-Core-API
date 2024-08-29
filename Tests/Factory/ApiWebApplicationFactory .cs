using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Helpers;
using Microsoft.Extensions.Configuration;

namespace Tests.Factory { }

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public IConfiguration Configuration { get; private set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            //I'ev added a seperate integrationsettings.json file for the database connection string.
            //I am able to extend this, so I can test Database connection. Note: I've not written DB test cases for this project
            Configuration = new ConfigurationBuilder()
              .AddJsonFile("appSettings.json")
              .Build();

            config.AddConfiguration(Configuration);
        }).ConfigureServices(services =>
        {
            services.AddDbContext<DataContext>();

            //string secret = builder.GetSetting("Secret")!;

            //// Configure JWT authentication
            //var key = Encoding.ASCII.GetBytes(secret); 
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(x =>
            //{
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = true;
            //    x.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false
            //    };
            //});


            // Build the service provider
            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataContext>();

                db.Database.EnsureCreated();
            }
        });
    }
}

