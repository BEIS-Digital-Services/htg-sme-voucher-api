
using Beis.HelpToGrow.Api.Voucher.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.RegisterVoucherApiServices(builder.Configuration);


builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddApiVersioning(v => 
{ 
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);    
    v.ReportApiVersions = true;

    v.UseApiBehavior = false; // version everything by default
    
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

app.MapControllers();


app.MapApiHealthChecks();


app.Run();



