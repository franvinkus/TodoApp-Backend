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
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
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

app.UseCors("AllowAll");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TodoAppDbContext>();
    // Baris ini akan memaksa Entity Framework membuat database & tabel jika belum ada
    dbContext.Database.EnsureCreated();
}

app.Run();
