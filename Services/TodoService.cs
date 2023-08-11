using System.Text.Json;
using AutoMapper;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TodoGrpc.Data;
using TodoGrpc.Models;

namespace ToDoGrpc.Services;
class TodoService : ToDoIt.ToDoItBase
{
  private readonly ILogger<TodoService> _logger;
  private readonly AppDbContext _dbContext;
  private readonly IMapper _mapper;
  public TodoService(ILogger<TodoService> logger, AppDbContext dbContext, IMapper mapper)
  {
    _logger = logger;
    _dbContext = dbContext;
    _mapper = mapper;
  }

  public override async Task<CreateTodoResponse> CreateTodo(CreateTodoRequest request, ServerCallContext context)
  {
    if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
    {
      throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
    }
    var entity = _mapper.Map<ToDoItem>(request);
    await _dbContext.AddAsync(entity);
    await _dbContext.SaveChangesAsync();
    return await Task.FromResult(new CreateTodoResponse
    {
      Id = entity.Id
    });
  }

  public override async Task<ReadTodoResponse> ReadTodo(ReadTodoRequest request, ServerCallContext context)
  {
    if (request.Id <= 0) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Resource Id"));

    var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id)?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
      return await Task.FromResult(_mapper.Map<ReadTodoResponse>(todoItem));
  }

  public override async Task<ReadAllTodoResponse> ReadAllTodo(ReadAllTodoRequest request, ServerCallContext context)
  {
    var response = new ReadAllTodoResponse();
    var todoItem = await _dbContext.ToDoItems.ToListAsync() ?? throw new RpcException(new Status(StatusCode.NotFound, "No Task Found"));
        var todoList = _mapper.Map<List<ReadTodoResponse>>(todoItem);
      response.ToDo.AddRange(todoList);
      return await Task.FromResult(response);
  }

  public override async Task<DeleteTodoResponse> DeleteTodo(DeleteTodoRequest request, ServerCallContext context)
  {
    if (request.Id <= 0) throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid Resource Id"));

    var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    _dbContext.Remove(todoItem);
    _dbContext.SaveChanges();
    return await Task.FromResult(new DeleteTodoResponse()
    {
      Id = request.Id
    });
  }
  public override async Task<UpdateTodoResponse> UpdateTodo(UpdateTodoRequest request, ServerCallContext context)
  {
    if (request.Id <= 0 || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Description))
    {
      throw new RpcException(new Status(StatusCode.InvalidArgument, "You must supply a valid object"));
    }
    var todoItem = await _dbContext.ToDoItems.FirstOrDefaultAsync(x => x.Id == request.Id) ?? throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
    todoItem.Description = request.Description;
    todoItem.Title = request.Title;
    todoItem.ToDoStatus = request.Status;
    await _dbContext.SaveChangesAsync();
    return await Task.FromResult(new UpdateTodoResponse()
    {
      Id = request.Id
    });
  }
}