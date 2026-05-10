using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CCA.Models;

namespace CCA.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = serviceProvider.GetRequiredService<CCFADbContext>();

        // Ensure database is created & migrations applied
        if (context.Database.CanConnect())
        {
            await context.Database.MigrateAsync();
        }

        // ✅ NO USER SEEDING HERE. 
        // Accounts will ONLY be created via /Account/Register UI.
    }
}