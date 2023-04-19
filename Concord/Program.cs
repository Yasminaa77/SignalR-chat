using Concord.Models;
using Concord.Hubs;


using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
DotNetEnv.Env.Load();



var builder = WebApplication.CreateBuilder(args);

// service
var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
//Console.WriteLine($"Connection string: {connectionString}");

builder.Services.AddDbContext<DatabaseContext>(
    opt =>
    {
      opt.UseNpgsql(connectionString);
      if (builder.Environment.IsDevelopment())
      {
        opt
          .LogTo(Console.WriteLine, LogLevel.Information)
          .EnableSensitiveDataLogging()
          .EnableDetailedErrors();
      }
    }
);




builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSignalR();


var app = builder.Build();
app.MapControllers();


app.MapGet("/", () => "Hello World!");
app.MapHub<ChatHub>("/r/chat");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
