using Microsoft.Extensions.DependencyInjection;
using Backend.Handlers;

namespace Backend.Extensions
{
  public static class ServiceExtensions
  {
    public static IServiceCollection AddWebSocketManager<T>(this IServiceCollection services)
        where T : WebSocketHandler
    {
      services.AddTransient<T>();
      return services;
    }
  }
}