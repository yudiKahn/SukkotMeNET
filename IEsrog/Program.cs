using Microsoft.AspNetCore.Authorization;
using IEsrog;
using IEsrog.Configuration;
using IEsrog.Data.Interfaces;
using IEsrog.Data.Repositories;
using IEsrog.Models;
using IEsrog.Services;

namespace IEsrog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();

            builder.Configuration.AddJsonFile("Configuration/appsettings.json")
                .AddEnvironmentVariables();

            var appConfig = new ApplicationConfiguration();
            builder.Configuration.Bind(appConfig);
            builder.Services.AddSingleton(appConfig);


            builder.Services.AddScoped<IAuthorizationHandler, AuthorizationHandlerService>();
            builder.Services.AddAuthorization(config =>
            {
                config.AddPolicy(Constants.Policies.IsAdmin, policy => policy.Requirements.Add(new AdminRequirement()));
                config.AddPolicy(Constants.Policies.IsUser, policy => policy.Requirements.Add(new UserRequirement()));
                config.AddPolicy(Constants.Policies.IsGuest, policy => policy.Requirements.Add(new GuestRequirement()));
            });
            
            builder.Services.AddScoped<AppStateService>();
            builder.Services.AddScoped<MainStateService>();
            builder.Services.AddScoped<InvoiceService>();

            builder.Services.AddSingleton<IRepositoryService, RepositoryService>();
            builder.Services.AddSingleton<EmailService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
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