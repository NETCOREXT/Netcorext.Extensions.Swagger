using System.Text.RegularExpressions;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Builder;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, string documentRoute = "/docs", string swaggerJsonRoutePattern = "{documentName}/swagger.{json|yaml|yml}", string defaultVersion = "v1")
    {
        var app = (WebApplication)builder;

        documentRoute = $"/{documentRoute.Trim('/')}";

        app.MapSwagger($"{documentRoute}/{swaggerJsonRoutePattern}");

        var efp = new EmbeddedFileProvider(typeof(ApplicationBuilderExtensions).Assembly, typeof(ApplicationBuilderExtensions).Assembly.GetName().Name + ".wwwroot");

        app.MapGet($"{documentRoute}/{{*remainder}}", context =>
                                                      {
                                                          var path = (context.Request.Path.Value ?? "")
                                                             .Replace(documentRoute, "", StringComparison.CurrentCultureIgnoreCase);

                                                          if (string.IsNullOrWhiteSpace(path))
                                                          {
                                                              context.Response.Redirect($"{documentRoute}/index.html");

                                                              return Task.CompletedTask;
                                                          }

                                                          var fi = efp.GetFileInfo(path);

                                                          if (!fi.Exists)
                                                          {
                                                              context.Response.StatusCode = 404;

                                                              return Task.CompletedTask;
                                                          }

                                                          using var stream = fi.CreateReadStream();

                                                          using var sr = new StreamReader(stream);

                                                          var content = sr.ReadToEnd();

                                                          if (!path.Equals("/swagger-initializer.js", StringComparison.CurrentCultureIgnoreCase)) return context.Response.WriteAsync(content);

                                                          var swaggerDoc = swaggerJsonRoutePattern.Replace("{documentName}", defaultVersion, StringComparison.CurrentCultureIgnoreCase);

                                                          swaggerDoc = Regex.Replace(swaggerDoc, @"\{.*\}", "json", RegexOptions.IgnoreCase);

                                                          swaggerDoc = $"{documentRoute}/{swaggerDoc}";
                                                                     
                                                          content = Regex.Replace(content, @"url:.*,", "url: location.origin + '" + swaggerDoc + "',", RegexOptions.IgnoreCase);
                                                          
                                                          return context.Response.WriteAsync(content);
                                                      });

        return app;
    }
}