using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CoreLibrary.Api.Swagger
{
    public class DiscriminatorDefaultSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            var polyAttr = type.GetCustomAttribute<JsonPolymorphicAttribute>();
            if (polyAttr == null) return;

            var derivedTypes = type.GetCustomAttributes<JsonDerivedTypeAttribute>().ToList();
            if (derivedTypes.Count == 0) return;

            var firstDiscriminator = derivedTypes.First().TypeDiscriminator?.ToString();
            if (string.IsNullOrEmpty(firstDiscriminator)) return;

            if (schema.Properties.ContainsKey(polyAttr.TypeDiscriminatorPropertyName))
            {
                schema.Properties[polyAttr.TypeDiscriminatorPropertyName].Default = new Microsoft.OpenApi.Any.OpenApiString(firstDiscriminator);
            }
        }
    }
}
