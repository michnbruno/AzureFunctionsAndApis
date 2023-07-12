using Azure.Identity;
using ProductCatalog;
using ProductCatalog.Services;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

//if (builder.Environment.IsProduction())
//{
//    //builder
//    builder.Configuration.AddAzureKeyVault(
//        new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
//        new DefaultAzureCredential());
//}

builder.Host.ConfigureAppConfiguration((hostingContext, configBuilder) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
        return;

    configBuilder.AddEnvironmentVariables();
    configBuilder.AddAzureKeyVault(
        new Uri("https://<keyvault>.vault.azure.net/"),
        new DefaultAzureCredential());
});



var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

WebApplication app = builder.Build();

startup.Configure(app/*, app.Environment*/);

app.Run();



//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
