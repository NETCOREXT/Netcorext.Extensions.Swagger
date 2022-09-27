using System.Text.RegularExpressions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, Uri tokenUri, Action<SwaggerGenOptions>? setupAction = null)
    {
        services.AddSwaggerGen(options =>
                               {
                                   options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                                                                           {
                                                                               Type = SecuritySchemeType.Http,
                                                                               In = ParameterLocation.Header,
                                                                               Name = "Authorization",
                                                                               BearerFormat = "jwt",
                                                                               Scheme = "bearer"
                                                                           });
                                   
                                   options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                                                                           {
                                                                               Type = SecuritySchemeType.OAuth2,
                                                                               In = ParameterLocation.Header,
                                                                               BearerFormat = "jwt",
                                                                               Scheme = "bearer",
                                                                               Flows = new OpenApiOAuthFlows
                                                                                       {
                                                                                           Password = new OpenApiOAuthFlow
                                                                                                      {
                                                                                                          TokenUrl = tokenUri
                                                                                                      },
                                                                                           ClientCredentials = new OpenApiOAuthFlow
                                                                                                               {
                                                                                                                   TokenUrl = tokenUri
                                                                                                               }
                                                                                       }
                                                                           });

                                   options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                                  {
                                                                      {
                                                                          new OpenApiSecurityScheme
                                                                          {
                                                                              Reference = new OpenApiReference
                                                                                          {
                                                                                              Id = "Bearer",
                                                                                              Type = ReferenceType.SecurityScheme
                                                                                          }
                                                                          }, Array.Empty<string>()
                                                                      }
                                                                  });

                                   options.AddSecurityRequirement(new OpenApiSecurityRequirement
                                                                  {
                                                                      {
                                                                          new OpenApiSecurityScheme
                                                                          {
                                                                              Reference = new OpenApiReference
                                                                                          {
                                                                                              Id = "OAuth2",
                                                                                              Type = ReferenceType.SecurityScheme
                                                                                          }
                                                                          }, Array.Empty<string>()
                                                                      }
                                                                  });

                                   options.CustomSchemaIds(ReplaceCustomId);

                                   setupAction?.Invoke(options);
                               });
        
        return services;
    }

    private static string ReplaceCustomId(Type type)
    {
        var id = type.ToString();

        if (Regex.IsMatch(id, @"^[a-zA-Z0-9\.\-_]+$")) return id;
        
        id = Regex.Replace(id, @"[^a-zA-Z0-9\.\-_]", "_").Trim('_');

        return id;
    }
}