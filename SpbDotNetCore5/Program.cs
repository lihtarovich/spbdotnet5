using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DataAccessLayer;
using Microsoft.AspNetCore.Hosting;
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
                String logConfigFile = "nlog.config";
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    logConfigFile = "nlog.development.config";
            
                logger = NLog.Web.NLogBuilder.ConfigureNLog(logConfigFile).GetCurrentClassLogger();
                
                logger.Info("Starting service...");
                
                logger.Info("Loading configuration...");
                String workingDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(workingDirectory)
                    .AddJsonFile("appsettings.json", false, true);
                IConfiguration config = configBuilder.Build();
                
                logger.Info("Checking DB SCHEMA updates...");
                DbUpdater updater = new DbUpdater(logger, config);
                updater.CheckForUpdate();
                
                logger.Info("Starting web host...");
                CreateHostBuilder(args, config).Build().Run();
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

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration config) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { 
                    webBuilder.UseConfiguration(config);
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog();
    }
}