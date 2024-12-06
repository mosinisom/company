using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Backend.Models;

namespace Backend.Handlers;
public class WebSocketHandler
{
  private readonly IEmployeeService _employeeService;
  private readonly IDepartmentService _departmentService;
  private readonly IProjectService _projectService;
  private readonly IPositionService _positionService;
  private readonly ConcurrentDictionary<string, WebSocket> _sockets;

  public WebSocketHandler(
      IEmployeeService employeeService,
      IDepartmentService departmentService,
      IProjectService projectService,
      IPositionService positionService)
  {
    _employeeService = employeeService;
    _departmentService = departmentService;
    _projectService = projectService;
    _positionService = positionService;
    _sockets = new ConcurrentDictionary<string, WebSocket>();
  }

  public async Task HandleAsync(HttpContext context)
  {
    try
    {
      using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
      var socketId = Guid.NewGuid().ToString();
      _sockets.TryAdd(socketId, webSocket);

      try
      {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
          var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

          if (result.MessageType == WebSocketMessageType.Text)
          {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message: {message}");
            RequestMessage request = null;
            try
            {
              request = JsonSerializer.Deserialize<RequestMessage>(message);
              Console.WriteLine($"Action: {request.Action}");
              Console.WriteLine($"Data: {request.Data}");
            }
            catch (Exception ex)
            {
              Console.WriteLine($"Error deserializing RequestMessage: {ex.Message}");
              throw;
            }

            object response = null;
            try
            {
              switch (request.Action)
              {
                // GET запросы
                case "getEmployees":
                  response = await _employeeService.GetAllAsync();
                  break;
                case "getDepartments":
                  response = await _departmentService.GetAllAsync();
                  break;
                case "getProjects":
                  response = await _projectService.GetAllAsync();
                  break;
                case "getPositions":
                  response = await _positionService.GetAllAsync();
                  break;

                // CREATE запросы    
                case "createEmployee":
                  var newEmployee = request.Data.Deserialize<Employee>();
                  response = await _employeeService.CreateAsync(newEmployee);
                  break;
                case "createDepartment":
                  var newDepartmentDto = JsonSerializer.Deserialize<DepartmentDto>(request.Data.ToString(), new JsonSerializerOptions
                  {
                    PropertyNameCaseInsensitive = true
                  });
                  response = await _departmentService.CreateAsync(newDepartmentDto);
                  break;
                case "createProject":
                  var newProject = request.Data.Deserialize<Project>();
                  response = await _projectService.CreateAsync(newProject);
                  break;
                case "createPosition":
                  {
                    var newPosition = request.Data.Deserialize<Position>();
                    response = await _positionService.CreateAsync(newPosition);
                    break;
                  }

                // UPDATE запросы
                case "updateEmployee":
                  var updatedEmployee = request.Data.Deserialize<Employee>();
                  response = await _employeeService.UpdateAsync(updatedEmployee);
                  break;
                case "updateDepartment":
                  var updatedDepartmentDto = JsonSerializer.Deserialize<DepartmentDto>(request.Data.ToString(), new JsonSerializerOptions
                  {
                    PropertyNameCaseInsensitive = true
                  });
                  response = await _departmentService.UpdateAsync(updatedDepartmentDto);
                  break;
                case "updateProject":
                  var updatedProject = request.Data.Deserialize<Project>();
                  response = await _projectService.UpdateAsync(updatedProject);
                  break;
                case "updatePosition":
                  var updatedPosition = request.Data.Deserialize<Position>();
                  response = await _positionService.UpdateAsync(updatedPosition);
                  break;

                // DELETE запросы    
                case "deleteEmployee":
                  await _employeeService.DeleteAsync(request.Id.Value);
                  response = true;
                  break;
                case "deleteDepartment":
                  await _departmentService.DeleteAsync(request.Id.Value);
                  response = true;
                  break;
                case "deleteProject":
                  await _projectService.DeleteAsync(request.Id.Value);
                  response = true;
                  break;
                case "deletePosition":
                  await _positionService.DeleteAsync(request.Id.Value);
                  response = true;
                  break;

                default:
                  throw new Exception($"Unknown action: {request.Action}");
              }

              var responseMessage = new ResponseMessage
              {
                Action = request.Action,
                Data = response,
                Success = true
              };

              var responseJson = JsonSerializer.Serialize(responseMessage);
              var responseBytes = Encoding.UTF8.GetBytes(responseJson);
              await webSocket.SendAsync(
                  new ArraySegment<byte>(responseBytes),
                  WebSocketMessageType.Text,
                  true,
                  CancellationToken.None);
            }
            catch (Exception ex)
            {
              var errorResponse = new ResponseMessage
              {
                Action = "error",
                Success = false,
                Message = ex.Message
              };
              var errorJson = JsonSerializer.Serialize(errorResponse);
              var errorBytes = Encoding.UTF8.GetBytes(errorJson);
              await webSocket.SendAsync(
                  new ArraySegment<byte>(errorBytes),
                  WebSocketMessageType.Text,
                  true,
                  CancellationToken.None);
            }
          }
        }
      }
      finally
      {
        WebSocket dummy;
        _sockets.TryRemove(socketId, out dummy);
        await webSocket.CloseAsync(
            WebSocketCloseStatus.NormalClosure,
            "Socket connection closed",
            CancellationToken.None);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"WebSocket error: {ex.Message}");
      throw;
    }
  }
}