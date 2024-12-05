using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Backend.Handlers;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Backend;
using Backend.Handlers;
using Backend.Extensions;

namespace Backend
{
  public class Startup
  {
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddScoped<IEmployeeService, EmployeeService>();
      services.AddScoped<IDepartmentService, DepartmentService>();
      services.AddScoped<IPositionService, PositionService>();
      services.AddScoped<IProjectService, ProjectService>();

      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseNpgsql(_configuration.GetConnectionString("DefaultConnection")));

      // Добавляем WebSocket middleware
      services.AddWebSocketManager<WebSocketHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebSocketHandler webSocketHandler)
    {
      app.UseWebSockets(new WebSocketOptions
      {
        KeepAliveInterval = TimeSpan.FromMinutes(2)
      });

      app.Use(async (context, next) =>
      {
        if (context.Request.Path == "/ws")
        {
          if (context.WebSockets.IsWebSocketRequest)
          {
            await webSocketHandler.HandleAsync(context);
          }
          else
          {
            context.Response.StatusCode = 400;
          }
        }
        else
        {
          await next();
        }
      });

    app.UseFileServer();  
    }
  }
}
