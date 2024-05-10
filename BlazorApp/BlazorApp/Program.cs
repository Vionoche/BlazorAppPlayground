using BlazorApp.BackgroundTasks;
using BlazorApp.Components;
using BlazorApp.Components.Shared;
using BlazorApp.Lib.Components;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using _Imports = BlazorApp.Client._Imports;

namespace BlazorApp;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .CreateBootstrapLogger();
        
        try
        {
            var app = CreateWebApplication(args);
            
            Log.Information("Application user name: " + GetApplicationUserName());
            
            await app.RunAsync();
            
            Log.Information("Host terminated clearly");
            
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return -1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static WebApplication CreateWebApplication(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();
        
        var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.UseConfiguration(configuration);
        
        builder.Host.UseSerilog((context, services, serilogConfiguration) =>
        {
            serilogConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        });

        builder.Services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddControllersWithViews();
        
        builder.Services.AddHostedService<TimedHostedService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(_Imports).Assembly);

        app.MapDefaultControllerRoute();
        
        app.MapGet("/shared", () => new RazorComponentResult<SharedBlazorComponent>());
        app.MapGet("/enhanced-shared", () => new RazorComponentResult<EnhancedSharedBlazorPage>());

        return app;
    }

    private static string GetApplicationUserName()
    {
        return string.Join("\\", Environment.UserDomainName, Environment.UserName);
    }
}