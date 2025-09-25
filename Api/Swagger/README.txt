Startup
.
.
public void ConfigureServices(IServiceCollection services)
{
    .
    services.AddSwaggerGen(x =>
    {
        .
        .
        x.SchemaFilter<DiscriminatorDefaultSchemaFilter>();
        .
    }
}