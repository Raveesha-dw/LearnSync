using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using WebApi.Data;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;

                // for email confirmation
                options.SignIn.RequireConfirmedPhoneNumber = true;
            })
            .AddRoles<IdentityRole>() // to be able to add roles
            .AddRoleManager<RoleManager<IdentityRole>>()
            .AddEntityFrameworkStores<DBContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddUserManager<UserManager<User>>() // make use of UserManager to create users
            .AddDefaultTokenProviders(); // to create tokens for email confirmation

            return services;
        }

        public static IServiceCollection ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<JWTService>(); // Register JWTService

            return services;
        }

        public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // validate the token based on the key we have provided inside appsettings.development.json JWT:Key
                        ValidateIssuerSigningKey = true,
                        //the issuer signing key based on JWT:Key
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"])),
                        // the issuer which in here is the api project url we are using
                        ValidIssuer = configuration["JWT:Issuer"],
                        // validate the issuer (whoever is issuing the JWT)
                        ValidateIssuer = true,
                        // don't validate audience (Angular side)
                        ValidateAudience = false,
                    };
                });

            return services;
        }
    }
}
