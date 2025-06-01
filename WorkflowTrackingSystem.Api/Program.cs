using Microsoft.OpenApi.Models;
using WorkflowTrackingSystem.Api.Data;
using WorkflowTrackingSystem.Api.Helpers;
using WorkflowTrackingSystem.Api.Middleware;
using WorkflowTrackingSystem.Api.Services.Implementations;
using WorkflowTrackingSystem.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Or .UseSqlite("DataSource=app.db")

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

// Register Services
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IProcessService, ProcessService>();
builder.Services.AddHttpClient<IValidationService, ValidationService>(); // Use HttpClientFactory for IValidationService

// Configure Validation Rules
builder.Services.Configure<ValidationRulesConfig>(builder.Configuration.GetSection("ValidationRules"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Workflow Tracking System API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200") // Angular dev server
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Workflow Tracking System API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });

    // Apply migrations automatically in development
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowSpecificOrigin"); // Use the CORS policy

app.UseAuthorization();

// Custom exception handling middleware
app.UseMiddleware<ExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();