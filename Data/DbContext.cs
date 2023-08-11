using Microsoft.EntityFrameworkCore;
using TodoGrpc.Models;

namespace TodoGrpc.Data;

public class AppDbContext : DbContext
{
  public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
  {
  }
  public DbSet<ToDoItem> ToDoItems=> Set<ToDoItem>();
}