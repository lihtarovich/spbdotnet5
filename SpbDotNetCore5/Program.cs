using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace SpbDotNetCore5
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            Logger logger = null;
            try
            {
                if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is Object)
                {
                    //here is docker-specific
                }
                
                String logConfigFile = "nlog.config";
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    logConfigFile = "nlog.development.config";
            
                String workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var logPath = Path.Combine(workingDirectory, logConfigFile);
                logger = NLog.Web.NLogBuilder.ConfigureNLog(logPath).GetCurrentClassLogger();
                
                logger.Info("Starting service...");
                logger.Info($"Working directory is {workingDirectory}");
                logger.Info("Loading configuration...");
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(workingDirectory)
                    .AddJsonFile("appsettings.json", false, true);
                IConfiguration config = configBuilder.Build();
                
                logger.Info("Checking DB SCHEMA updates...");
                DbUpdater updater = new DbUpdater(logger, config);
                updater.CheckForUpdate();
                
                logger.Info("Starting web host...");
                CreateHostBuilder(args, config, workingDirectory).Build().Run();
            }
            catch (Exception e)
            {
                logger?.Error(e, "Shutting down because of exception");
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config, String workingDir) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { 
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>  ConfigureEndpoints(config, options, workingDir));
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
        
        public static void ConfigureEndpoints(IConfiguration configuration, KestrelServerOptions options, String workingDir)
        {
            var certPassword = configuration?.GetSection("Kestrel:Certificates:Default")?["Password"]; 
            var certPath = configuration?.GetSection("Kestrel:Certificates:Default")?["Path"];
            var realPath = Path.Combine(workingDir, certPath);
            var certificate = new X509Certificate2 ( realPath , certPassword );
            options.ConfigureHttpsDefaults(options => options.ServerCertificate = certificate);
        }
    }
}