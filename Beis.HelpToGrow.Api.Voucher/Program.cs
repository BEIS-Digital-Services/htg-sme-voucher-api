using Beis.HelpToGrow.Api.Voucher.Extensions;
using Microsoft.AspNetCore.Mvc.Versioning;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppConfig");

if (connectionString != null)
{
    builder.Services.AddAzureAppConfiguration();
    
    builder.Host.ConfigureAppConfiguration(configBuilder =>
    {
        configBuilder.AddAzureAppConfiguration(connectionString);
    });
}

// **************** Adding cors for local development - Cors will be handled  ************** //
// **************** by the app service in azure for other environments *********************** //

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(
            policy => { policy.WithOrigins().AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
    });
}

// **************************************************************************************** //

// Add services to the container.
builder.Services.RegisterVoucherApiServices(builder.Configuration);

builder.Services.AddControllers();

//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddApiVersioning(o => 
{ 
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ApiVersionSelector = new CurrentImplementationApiVersionSelector(o);
    o.DefaultApiVersion = ApiVersion.Default; // new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);    
    o.ReportApiVersions = true;

    o.UseApiBehavior = false; // version everything by default
    
});


var app = builder.Build();


// this is needed due to changes in the way the date formats are handled  https://stackoverflow.com/questions/69961449/net6-and-datetime-problem-cannot-write-datetime-with-kind-utc-to-postgresql-ty
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json","beis-htg-sme-voucher-service v1"));

app.UseHttpsRedirection();

// ********* Adding cors for local development *********************** //
if (builder.Environment.IsDevelopment())
{
    app.UseCors();
}

app.MapControllers();


app.MapApiHealthChecks();


app.Run();



