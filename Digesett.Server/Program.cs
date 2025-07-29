using Digesett.Server.Data;
using Digesett.Server.Services;
using Digesett.Server.Services.EmployeeService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpClient();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//Inyecto los servicios de Employee donde esta la integracion con el sistema de nomina.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IServiceCommon, ServiceCommon>();



var app = builder.Build();

//Configuracion de los CORS.
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials()
);

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();




app.UseAuthorization();

app.MapControllers();

app.Run();
