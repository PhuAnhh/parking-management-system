using Microsoft.EntityFrameworkCore;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Persistence.Repositories;
using Final_year_Project.Application.Repositories;
using Final_year_Project.Application.Services;
using Final_year_Project.Application.Services.Abstractions;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Environment.IsDevelopment()
    ? builder.Configuration.GetConnectionString("LocalConnection")
    : builder.Configuration.GetConnectionString("RemoteConnection");

builder.Services.AddDbContext<DeviceServiceContext>(options =>
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();