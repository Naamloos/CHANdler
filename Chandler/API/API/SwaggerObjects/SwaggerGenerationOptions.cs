using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.SwaggerObjects
{
    public class SwaggerGenerationOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public SwaggerGenerationOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var baseVersionInfo = new OpenApiInfo()
                {
                    Title = "CHANdler API",
                    Version = description.ApiVersion.ToString(),
                    Description = "Provides API endpoints for CHANdler.",
                    Contact = new OpenApiContact()
                    {
                        Name = "Github Page",
                        Url = new("https://github.com/Naamloos/Chandler")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "MIT License",
                        Url = new("https://opensource.org/licenses/MIT")
                    }
                };

                if (description.IsDeprecated)
                    baseVersionInfo.Description += "\nThis version has been deprecated";

                options.SwaggerDoc(description.GroupName, baseVersionInfo);
            }
        }
    }
}
