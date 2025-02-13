using FlowerFarmTaskManagementSystem.BusinessLogic.IService;
using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using FlowerFarmTaskManagementSystem.BusinessObject.Mapper;
using FlowerFarmTaskManagementSystem.DataAccess;
using FlowerFarmTaskManagementSystem.DataAccess.IRepositories;
using FlowerFarmTaskManagementSystem.DataAccess.Repositories;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddOData(options => options.Select().Expand().Filter().OrderBy().Count().SetMaxTop(100));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IFarmToolCategoriesService, FarmToolCategoriesService>();
builder.Services.AddScoped<IFarmToolsService, FarmToolsService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

//builder.Services.AddDbContext<FlowerFarmTaskManagementSystemDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FlowerFarmTaskManagementSystemDbContext>(options =>
	options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 32))));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
