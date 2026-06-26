using Microsoft.AspNetCore.Identity;

namespace Achievement.Data.Seed
{
    /// <summary>
    /// Extensão para inicializar a base de dados com dados iniciais.
    /// https://github.com/IPT-DW-2025-2026/tB-API/blob/main/tB-Fotografias/Data/Seed/DbInitializerExtension.cs
    /// </summary>
    internal static class DbInitializerExtension
    {
        public static async Task<IApplicationBuilder> UseItToSeedSqlServer(this IApplicationBuilder app)
        {
            ArgumentNullException.ThrowIfNull(app, nameof(app));

            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                await DbInitializer.Initialize(context, userManager, roleManager);
            }
            catch (Exception ex)
            {

            }

            return app;
        }
    }
}
