using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BuildingBlocks.Extentions
{
    public static class AuthorizationExtention
    {
        public static void AddAuthorizationServices(this WebApplicationBuilder builder)
        {
            string issuer = builder.Configuration["Jwt:Issuer"] ?? throw new RequiredConfigurationException("Jwt:Issuer");
            List<string> audiences = builder.Configuration.GetSection("Jwt:Audiences").Get<List<string>>()
                                    ?? throw new RequiredConfigurationException("Jwt:Audiences");
            string key = builder.Configuration["Jwt:Key"] ?? throw new RequiredConfigurationException("Jwt:Key");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = issuer,
                           ValidAudiences = audiences,
                           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                       };
                   });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Authenticated", policy => policy.RequireAuthenticatedUser());
            });

        }

        public static void UseAuthorizationServices(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }
    }
}