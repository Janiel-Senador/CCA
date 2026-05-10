using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CCA.Data;

// PDF: IDesignTimeDbContextFactory for migration support
public class CCFADbContextFactory : IDesignTimeDbContextFactory<CCFADbContext>
{
    public CCFADbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        // Configure DbContext options
        var optionsBuilder = new DbContextOptionsBuilder<CCFADbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        );

        return new CCFADbContext(optionsBuilder.Options);
    }
}