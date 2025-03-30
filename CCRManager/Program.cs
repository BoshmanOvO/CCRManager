using CCRManager.Models;
using CCRManager.Services;
using CCRManager.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Add Services to the container
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAcrTokenProvider, AcrTokenProvider>();
builder.Services.AddScoped<ICommonContainerRegistryServices, CommonContainerRegistryServices>();


builder.Services.AddControllers();

// add JWT settings
//var jwtsettings = new JwtSettings();
//builder.Configuration.GetSection("JwtSettings").Bind(jwtsettings);
//builder.Services.AddSingleton(jwtsettings);

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes((jwtsettings.SecretKey)))
//    };
//});

//builder.Services.AddAuthorization(options =>
//{
//    // Optionally, you can add custom policies here
//    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
//});


//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
//    {
//        Title = "Auth Demo",
//        Version = "v1"
//    });
//    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter the token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });

//    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                }
//            },
//            []
//        }
//    });
//});

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
