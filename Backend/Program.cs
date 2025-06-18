using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Persistence.Repositories;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services;
using Final_year_Project.Application.Services.Abstractions;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Final_year_Project.Api.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ParkingManagementContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddScoped<IComputerService, ComputerService>();
builder.Services.AddScoped<IControlUnitService, ControlUnitService>();
builder.Services.AddScoped<IGateService, GateService>();
builder.Services.AddScoped<ILaneService, LaneService>();
builder.Services.AddScoped<ILedService, LedService>();

builder.Services.AddScoped<ICustomerGroupService, CustomerGroupService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICardGroupService, CardGroupService>();
builder.Services.AddScoped<ICardService, CardService>();

builder.Services.AddScoped<IEntryLogService, EntryLogService>();
builder.Services.AddScoped<IExitLogService, ExitLogService>();
builder.Services.AddScoped<IRevenueReportService, RevenueReportService>();
builder.Services.AddScoped<IWarningEventService, WarningEventService>();

builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                         .AllowAnyMethod()
                         .AllowAnyHeader());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();