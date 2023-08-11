using AutoMapper;
using TodoGrpc.Models;

namespace ToDoGrpc.Mapper;

class TodoProfile:Profile{
  public TodoProfile()
  {
    CreateMap<ToDoItem,CreateTodoRequest>().ReverseMap();
    CreateMap<ToDoItem,ReadTodoResponse>()
    .ForMember(dest => dest.Status,opt=>opt.MapFrom(src=>src.ToDoStatus))
    .ReverseMap();
    
  }
}