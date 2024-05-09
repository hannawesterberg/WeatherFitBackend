using Microsoft.EntityFrameworkCore;
using WeatherFit.Data;
using System.Text.Json.Serialization; // Used to configure JsonSerializerOptions to handle object cycles
using WeatherFit.Services;

var builder = WebApplication.CreateBuilder(args);

//1. Database context setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add HttpClient for WeatherService
builder.Services.AddHttpClient<WeatherService>(); // This line adds the HttpClient service for WeatherService

//3. Add BusinessLogic sevice 
builder.Services.AddScoped<BusinessLogic>(); // Adds BusinessLogic to the DI container

//4.  Adds MVC controllers to the DI container, enables app to handle API request by finding appropriate controllers
builder.Services.AddLogging();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

//5. CORS Setup for Frontend connection 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Swagger generation 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Logging configuration
builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();  
builder.Logging.AddDebug();  


var app = builder.Build();

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularDevOrigin");

app.MapControllers();

app.Run();


