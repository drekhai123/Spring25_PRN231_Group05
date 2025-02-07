using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;


namespace FlowerFarmTaskManagementSystem.DataAccess
{
    public class FlowerFarmTaskManagementSystemDbContext : DbContext
    {
        public FlowerFarmTaskManagementSystemDbContext(DbContextOptions<FlowerFarmTaskManagementSystemDbContext> options)
                 : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<ProductField> ProductFields { get; set; }
        public DbSet<TaskWork> TaskWorks { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }


    }
}
