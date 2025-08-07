using Digesett.Server.Data;
using Digesett.Server.Services;
using Digesett.Server.Services.EmployeeService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var corsPolicyName = "CorsPolicy";
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: corsPolicyName,
        policy => {
            policy.WithOrigins("http://172.16.0.18:2050","http://localhost:5069")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});


builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Digesett.Server", Version = "v1" });
});


builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//Inyecto los servicios de Employee donde esta la integracion con el sistema de nomina.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IServiceCommon, ServiceCommon>();

var app = builder.Build();

app.UseCors(corsPolicyName);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Digesett.Server v1");
});

app.UseAuthorization();
app.MapControllers();

app.Run();
