namespace Netcorext.Extensions.Swagger.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder builder, string name, string documentUrl)
    {
        var app = (WebApplication)builder;
        
        app.MapSwagger(documentUrl + "/{documentName}/swagger.json");

        app.UseSwaggerUI(options =>
                         {
                             options.SwaggerEndpoint("v1/swagger.json", name);
                             options.RoutePrefix = documentUrl;
                         });

        return app;
    }
}