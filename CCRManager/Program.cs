using CommonContainerRegistry;
using CommonContainerRegistry.Services;
using CommonContainerRegistry.Services.ServicesInterfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Refit;

var builder = WebApplication.CreateBuilder(args);

// Add Services to the container
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAcrTokenProvider, AcrTokenProvider>();
builder.Services.AddScoped<ICommonContainerRegistryServices, CommonContainerRegistryServices>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("Azure"));

builder.Services.AddRefitClient<IAzureApiService> ().ConfigureHttpClient(c => c.BaseAddress = new Uri("https://management.azure.com"));



builder.Services.AddControllers();


// register Auth service that handles JWT creation and validation



// Add config for JWT bearer token.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "Auth Demo",
        Version = "v1"
    });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter the token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
