using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FlowerFarmTaskManagementSystem.DataAccess
{
	public class FlowerFarmTaskManagementSystemDbContextFactory : IDesignTimeDbContextFactory<FlowerFarmTaskManagementSystemDbContext>
	{
		public FlowerFarmTaskManagementSystemDbContext CreateDbContext(string[] args)
		{
			IConfigurationRoot configuration = new ConfigurationBuilder()
				.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FlowerFarmTaskManagementSystem.API"))
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<FlowerFarmTaskManagementSystemDbContext>();
			var connectionString = configuration.GetConnectionString("DefaultConnection");

			optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32)));

			return new FlowerFarmTaskManagementSystemDbContext(optionsBuilder.Options);
		}
	}
}
