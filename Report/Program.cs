using Microsoft.OpenApi.Models;
using report._Common;
using Swashbuckle.AspNetCore.Filters;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

Helper.Configuration = configuration;

var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Add services to the container.

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IKeyManager, KeyManager>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Report API - Version 1", Version = "v1.0" });
    c.SwaggerDoc("v2", new OpenApiInfo() { Title = "Report API - Version 2", Version = "v2.0" });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    //c.EnableAnnotations();
    // c.DocumentFilter<DocumentFilter>();
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'APIKey' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'APIKey 12345abcdef'",
        Name = "APIKey",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityDefinition("APIVersion", new OpenApiSecurityScheme
    {
        Description = @"Client Version",
        Name = "APIVersion",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "APIVersion"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                      new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                        },
                        new List<string>()
                    }
            });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                    {
                      new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "APIVersion"
                            },
                        },
                        new List<string>()
                    }
            });
    c.OperationFilter<SecurityRequirementsOperationFilter>();
    //  c.SchemaFilter<SchemaFilter>();

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
