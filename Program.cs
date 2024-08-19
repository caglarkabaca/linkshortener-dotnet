using System.Text;
using LinkShortenerAPI.Hubs;
using LinkShortenerAPI.Models;
using LinkShortenerAPI.Models.DTO;
using LinkShortenerAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});
builder.Services.AddCors(
    options => options.AddPolicy("AllowAll", builder =>
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    ));
builder.Services.AddMvc();

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSignalR();

// JWT
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()!;
// builder.Services.AddScoped<AuthHandler>();
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<JwtBearerOptions, JwtBearerHandler>(
        JwtBearerDefaults.AuthenticationScheme, x =>
        {
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience
            };

            x.Events = new JwtBearerEvents()
            {
                OnChallenge = context =>
                {
                    context.Response.OnStarting(async () =>
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(new ErrorResponseDTO
                        {
                            ErrorMessage = "Bu sayfayı görünteleyebilmek için yetkilendirme gerekli."
                        }.ToString());
                    });
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    context.Response.OnStarting(async () =>
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(new ErrorResponseDTO
                        {
                            ErrorMessage = "Bu sayfayı görünteleyebilmek için yeterli yetkiniz yok!"
                        }.ToString());
                    });
                    return Task.CompletedTask;
                }
            };
        });
builder.Services.AddSingleton<JwtOptions>(jwtOptions);

// \JWT
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<CouponService>();


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.UseMiddleware<AuthMiddleware>();

app.MapControllers();
app.MapHub<MainHub>("/hub");

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.Run();