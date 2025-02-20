using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoreLibrary.Swagger
{
    public class SwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties == null)
                return;

            if (schema.Properties.TryGetValue("type", out var typeProperty))
            {
                schema.Properties.Remove("type");
                schema.Properties = new Dictionary<string, OpenApiSchema>
                {
                    { "type", typeProperty }
                }
                .Concat(schema.Properties)
                .ToDictionary(p => p.Key, p => p.Value);
            }
        }
    }
}
