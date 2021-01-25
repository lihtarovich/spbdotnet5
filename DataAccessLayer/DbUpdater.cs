using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using NLog;

namespace DataAccessLayer
{
    public class DbUpdater
    {
        private readonly Logger _logger;
        private DbContextOptions<UserContext> _options;
        public DbUpdater(Logger logger, IConfiguration configuration)
        {
            _logger = logger;
            var optionsBuilder = new DbContextOptionsBuilder<UserContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            _options = optionsBuilder.Options;
        }
        
        public void CheckForUpdate()
        {
            try
            {
                _logger.Info("[DB] Checking for dbupdate...");
                var db = new UserContext(_options);
                List<String> pendingMigrations = db.Database.GetPendingMigrations().ToList();
                if (pendingMigrations.Any())
                {
                    _logger.Info("[DB] Founded pending migrations. Starting database upgrade...");
                    var migrator = db.Database.GetService<IMigrator>();
                    foreach (var targetMigration in pendingMigrations)
                        migrator.Migrate(targetMigration);
                    _logger.Info("[DB] Database upgrade completed");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[DB] Database upgrade failed");
                throw;
            }
        }
    }
}