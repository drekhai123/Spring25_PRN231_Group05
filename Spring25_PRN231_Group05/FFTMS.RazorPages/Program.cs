using FlowerFarmTaskManagementSystem.BusinessLogic.Service;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); // Removed the AddPageRoute configuration
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Redirect unauthenticated users to /Auth/LoginPage
//app.Use(async (context, next) =>
//{
//    if (context.Request.Path == "/" && !context.Request.Cookies.ContainsKey("AuthToken"))
//    {
//        context.Response.Redirect("/Auth/LoginPage");
//        return;
//    }
//    await next();
//});

app.MapRazorPages();
app.MapHub<ProductFieldHub>("/productFieldHub");
app.Run();