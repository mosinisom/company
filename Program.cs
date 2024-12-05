using Backend;
using Backend.Data;
using Microsoft.EntityFrameworkCore;

public class Program
{
  public static void Main(string[] args)
  {
    var host = CreateHostBuilder(args).Build();

    using (var scope = host.Services.CreateScope())
    {
      var services = scope.ServiceProvider;
      try
      {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Только применяем миграции, без инициализации данных
        context.Database.Migrate();
      }
      catch (Exception ex)
      {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при миграции базы данных");
      }
    }

    host.Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          });
}
