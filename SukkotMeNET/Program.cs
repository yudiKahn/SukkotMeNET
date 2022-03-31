using MudBlazor.Services;
using SukkotMeNET.Configuration;
using SukkotMeNET.Data;
using SukkotMeNET.Interfaces;
using SukkotMeNET.Models;
using SukkotMeNET.Services;

namespace SukkotMeNET
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();

            builder.Configuration.AddJsonFile("Configuration/appsettings.json");

            builder.Services.Configure<MongodbConfig>(builder.Configuration.GetSection(nameof(MongodbConfig)));

            builder.Services.AddSingleton<IRepositoryService, RepositoryService>();


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

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}