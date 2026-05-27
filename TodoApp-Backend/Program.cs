using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Todo.Entities;
using TodoApp_Backend.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkSqlServer();
builder.Services.AddDbContextPool<TodoAppDbContext>(options =>
{
    var constring = configuration.GetConnectionString("TodoDb");
    options.UseSqlServer(constring);
});

builder.Services.AddTransient<TodoServices>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
