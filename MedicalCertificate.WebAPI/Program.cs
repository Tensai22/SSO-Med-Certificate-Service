using MedicalCertificate.Application;
using MedicalCertificate.Application.Interfaces;
using MedicalCertificate.Domain.Options;
using MedicalCertificate.Application.Services;
using MedicalCertificate.Infrastructure.Services;
using MedicalCertificate.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using MedicalCertificate.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IFileStorageService, MinioFileStorageService>();

builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection("Minio"));
builder.Services.AddSingleton<IFileStorageService, MinioFileStorageService>(); 

builder.Services.AddControllers();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
builder.Services.AddScoped<ICertificateStatusHistoryRepository, CertificateStatusHistoryRepository>();
builder.Services.AddScoped<ICertificateStatusRepository, CertificateStatusRepository>();
builder.Services.AddScoped<DbContext, AppDbContext>();

builder.Services.AddScoped<IFileRepository, FileRepository>();

builder.Services.AddApplicationServices();
builder.Services.AddDomain(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IJwtProvider, JwtProvider>();

builder.Services.Configure<JwtConfigurationOptions>(
    builder.Configuration.GetSection("JwtConfigurationOptions"));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtConfigurationOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtConfigurationOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtConfigurationOptions:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(o =>
{
    o.AddPolicy("all", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");

app.UseCors("all");

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestTimingMiddleware>();

app.MapControllers();

app.Run();