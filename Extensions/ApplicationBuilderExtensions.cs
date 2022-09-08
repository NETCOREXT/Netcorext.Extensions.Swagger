using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Netcorext.Extensions.Swagger.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, string name, string documentRoutePrefix = "/docs", string documentRoutePattern = "/docs/{*remainder}", string swaggerJsonRoutePattern = "/docs/{documentName}/swagger.{json|yaml}")
    {
        var app = (WebApplication)builder;

        app.MapSwagger(swaggerJsonRoutePattern);

        using var scope = app.Services.CreateScope();

        var options = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SwaggerUIOptions>>().Value;

        options.SwaggerEndpoint("v1/swagger.json", name);
        options.RoutePrefix = documentRoutePrefix.TrimStart('/');

        options.ConfigObject.Urls ??= new[] { new UrlDescriptor { Name = "V1 Docs", Url = "v1/swagger.json" } };

        var endpoints = (IEndpointRouteBuilder)app;

        var pipeline = endpoints.CreateApplicationBuilder()
                                .UseMiddleware<SwaggerUIMiddleware>(options)
                                .Build();

        endpoints.MapGet(documentRoutePattern, pipeline);

        return app;
    }
}