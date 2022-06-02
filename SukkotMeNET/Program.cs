using Microsoft.AspNetCore.Authorization;
using SukkotMeNET.Configuration;
using SukkotMeNET.Extensions;
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

            builder.Configuration.AddJsonFile("Configuration/appsettings.json");

            Console.WriteLine("Trying to get configuraion");
            builder.Services.Configure<MongodbConfig>(builder.Configuration.GetSection(nameof(MongodbConfig)));

            var emailConfig = new EmailConfig();
            builder.Configuration.GetSection(nameof(EmailConfig)).Bind(emailConfig);
            builder.Services.AddSingleton(emailConfig);
            Console.WriteLine($"Got config, address: {emailConfig.Address}");

            builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerService>();
            builder.Services.AddAuthorization(config =>
            {
                config.AddPolicy(Constants.Policies.IsAdmin, policy => policy.Requirements.Add(new AdminRequirement()));
                config.AddPolicy(Constants.Policies.IsUser, policy => policy.Requirements.Add(new UserRequirement()));
                config.AddPolicy(Constants.Policies.IsGuest, policy => policy.Requirements.Add(new GuestRequirement()));
            });

            builder.Services.AddSingleton<IRepositoryService, RepositoryService>();
            builder.Services.AddSingleton<AppStateService>();
            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddHostedService<MainService>();
            builder.Services.AddSingleton<MainService>();

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