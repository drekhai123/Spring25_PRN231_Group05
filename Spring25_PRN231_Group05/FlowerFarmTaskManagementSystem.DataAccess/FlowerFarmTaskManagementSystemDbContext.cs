using FlowerFarmTaskManagementSystem.BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace FlowerFarmTaskManagementSystem.DataAccess
{
    public class FlowerFarmTaskManagementSystemDbContext : DbContext
    {
			public FlowerFarmTaskManagementSystemDbContext(DbContextOptions<FlowerFarmTaskManagementSystemDbContext> options)
				: base(options)
			{
			}

			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				if (!optionsBuilder.IsConfigured)
				{
					var configuration = new ConfigurationBuilder()
						.SetBasePath(AppContext.BaseDirectory)
						.AddJsonFile("appsettings.json")
						.Build();

					var connectionString = configuration.GetConnectionString("DefaultConnection");
					optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)));
				}
			}

			public DbSet<User> Users { get; set; }
			public DbSet<Category> Categories { get; set; }
			public DbSet<Product> Products { get; set; }
			public DbSet<Field> Fields { get; set; }
			public DbSet<ProductField> ProductFields { get; set; }
			public DbSet<TaskWork> TaskWorks { get; set; }
			public DbSet<UserTask> UserTasks { get; set; }
		    public DbSet<Supplier> Suppliers { get; set; }
		    public DbSet<TypeSupplier> TypeSuppliers { get; set; }
			public DbSet<TypeOfSupplier> TypeOfSuppliers { get; set; }
			public DbSet<FarmToolCategories> FarmToolCategories { get; set; }
			public DbSet<FarmTools> FarmTools { get; set; }
			public DbSet<FarmToolsOfTask> FarmToolsOfTasks { get; set; }


	}
}
