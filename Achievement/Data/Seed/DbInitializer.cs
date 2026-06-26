using Achievement.Models;
using Achievement.ValidationFiles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Achievement.Data.Seed
{
    /// <summary>
    /// Preenche a base de dados com dados iniciais.
    /// https://github.com/IPT-DW-2025-2026/tB-API/blob/main/tB-Fotografias/Data/Seed/DbInitializer.cs
    /// </summary>
    internal class DbInitializer
    {
        internal static async Task Initialize(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
            dbContext.Database.EnsureCreated();

            // var auxiliar
            bool added = false;
            var now = DateTime.UtcNow;

            // ============== Géneros ==============
            var action = new Genre { Name = "Action" };
            var adventure = new Genre { Name = "Adventure" };
            var rpg = new Genre { Name = "RPG" };
            var shooter = new Genre { Name = "Shooter" };
            var platformer = new Genre { Name = "Platformer" };
            var strategy = new Genre { Name = "Strategy" };
            var simulation = new Genre { Name = "Simulator" };
            var sports = new Genre { Name = "Sports" };
            var racing = new Genre { Name = "Racing" };
            var puzzle = new Genre { Name = "Puzzle" };
            var horror = new Genre { Name = "Horror" };
            var fighting = new Genre { Name = "Fighting" };
            var vn = new Genre { Name = "Visual Novel" };
            var indie = new Genre { Name = "Indie" };
            var music = new Genre { Name = "Music" };

            if (!dbContext.Genres.Any())
            {
                dbContext.Genres.AddRange(action, adventure, rpg, shooter, platformer,
                strategy, simulation, sports, racing, puzzle, horror, fighting, vn, indie, music);

                added = true;
            }



            // ============== Plataformas ==============
            var pc = new Models.Platform { Type = PlatformType.PC, Name = "Windows PC" };
            var linux = new Models.Platform { Type = PlatformType.PC, Name = "Linux" };
            var mac = new Models.Platform { Type = PlatformType.PC, Name = "macOS" };
            var ps5 = new Models.Platform { Type = PlatformType.Console, Name = "PlayStation 5" };
            var ps4 = new Models.Platform { Type = PlatformType.Console, Name = "PlayStation 4" };
            var xboxSeries = new Models.Platform { Type = PlatformType.Console, Name = "Xbox Series X|S" };
            var xboxOne = new Models.Platform { Type = PlatformType.Console, Name = "Xbox One" };
            var switchConsole = new Models.Platform { Type = PlatformType.Portable, Name = "Nintendo Switch" };
            var switch2Console = new Models.Platform { Type = PlatformType.Portable, Name = "Nintendo Switch 2" };
            var steamDeck = new Models.Platform { Type = PlatformType.Portable, Name = "Steam Deck" };
            var android = new Models.Platform { Type = PlatformType.Mobile, Name = "Android" };
            var ios = new Models.Platform { Type = PlatformType.Mobile, Name = "iOS" };
            var quest = new Models.Platform { Type = PlatformType.VR, Name = "Meta Quest 3" };
            var steamVR = new Models.Platform { Type = PlatformType.VR, Name = "SteamVR" };

            if (!dbContext.Platforms.Any())
            {
                dbContext.Platforms.AddRange(pc, linux, mac, ps5, ps4, xboxSeries, xboxOne,
                switchConsole, switch2Console, steamDeck, android, ios, quest, steamVR);

                added = true;
            }


            // ============== Jogos ==============

            if(!dbContext.Games.Any())
            {
                dbContext.Games.AddRange(
                    new Game
                    {
                        Name = "The Witcher 3: Wild Hunt",
                        Description = "An open-world RPG following monster hunter Geralt of Rivia as he searches for his adopted daughter across a war-torn continent.",
                        ReleaseDate = new DateTime(2015, 5, 19),
                        CreatedAt = now,
                        Length = 100,
                        Developer = "CD Projekt Red",
                        Slug = "the-witcher-3-wild-hunt",
                        BannerImage = "images/games/banners/the-witcher-3-banner.png",
                        CoverImage = "images/games/covers/the-witcher-3-cover.png",
                        Genres = new List<Genre> { rpg, action, adventure },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries, xboxOne, switchConsole }
                    },
                    new Game
                    {
                        Name = "Elden Ring",
                        Description = "A vast action RPG set in the Lands Between, where players journey to become the Elden Lord across a brutal open world.",
                        ReleaseDate = new DateTime(2022, 2, 25),
                        CreatedAt = now,
                        Length = 60,
                        Developer = "FromSoftware",
                        Slug = "elden-ring",
                        BannerImage = "images/games/banners/elden-ring-banner.png",
                        CoverImage = "images/games/covers/elden-ring-cover.png",
                        Genres = new List<Genre> { rpg, action },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries, xboxOne }
                    },
                    new Game
                    {
                        Name = "Dark Souls III",
                        Description = "Set in the decaying kingdom of Lothric, players take on the role of the \"Ashen One,\" an undead warrior tasked with rekindling the First Flame to prevent the world from falling into darkness. The game features challenging combat, intricate level design, and a deep, lore-rich narrative.",
                        ReleaseDate = new DateTime(2016, 3, 24),
                        CreatedAt = now,
                        Length = 88,
                        Developer = "FromSoftware",
                        Slug = "dark-souls-iii",
                        BannerImage = "images/games/banners/dark-souls-iii-banner.png",
                        CoverImage = "images/games/covers/dark-souls-iii-cover.png",
                        Genres = new List<Genre> { rpg, action },
                        Platforms = new List<Platform> { pc, ps4, xboxOne }
                    },
                    new Game
                    {
                        Name = "Red Dead Redemption 2",
                        Description = "An epic tale of life in America's unforgiving heartland, following outlaw Arthur Morgan and the Van der Linde gang.",
                        ReleaseDate = new DateTime(2018, 10, 26),
                        CreatedAt = now,
                        Length = 80,
                        Developer = "Rockstar Games",
                        Slug = "red-dead-redemption-2",
                        BannerImage = "images/games/banners/red-dead-redemption-2-banner.png",
                        CoverImage = "images/games/covers/red-dead-redemption-2-cover.png",
                        Genres = new List<Genre> { action, adventure },
                        Platforms = new List<Platform> { pc, ps4, xboxOne }
                    },
                    new Game
                    {
                        Name = "The Legend of Zelda: Breath of the Wild",
                        Description = "An open-air adventure across the kingdom of Hyrule, where Link awakens from a hundred-year slumber to defeat Calamity Ganon.",
                        ReleaseDate = new DateTime(2017, 3, 3),
                        CreatedAt = now,
                        Length = 50,
                        Developer = "Nintendo",
                        Slug = "the-legend-of-zelda-breath-of-the-wild",
                        BannerImage = "images/games/banners/breath-of-the-wild-banner.png",
                        CoverImage = "images/games/covers/breath-of-the-wild-cover.png",
                        Genres = new List<Genre> { adventure, action },
                        Platforms = new List<Platform> { switchConsole, switch2Console}
                    },
                    new Game
                    {
                        Name = "Counter-Strike 2",
                        Description = "The premier competitive tactical shooter, pitting two teams against each other in objective-based rounds.",
                        ReleaseDate = new DateTime(2023, 9, 27),
                        CreatedAt = now,
                        Length = 361,
                        Developer = "Valve",
                        Slug = "counter-strike-2",
                        BannerImage = "images/games/banners/counter-strike-2-banner.png",
                        CoverImage = "images/games/covers/counter-strike-2-cover.png",
                        Genres = new List<Genre> { shooter, action },
                        Platforms = new List<Platform> { pc, steamDeck }
                    },
                    new Game
                    {
                        Name = "Hollow Knight",
                        Description = "A challenging hand-drawn action-adventure through the ruined insect kingdom of Hallownest.",
                        ReleaseDate = new DateTime(2017, 2, 24),
                        CreatedAt = now,
                        Length = 40,
                        Developer = "Team Cherry",
                        Slug = "hollow-knight",
                        BannerImage = "images/games/banners/hollow-knight-banner.png",
                        CoverImage = "images/games/covers/hollow-knight-cover.png",
                        Genres = new List<Genre> { indie, platformer, adventure, action },
                        Platforms = new List<Platform> { pc, ps4, xboxOne, switchConsole, steamDeck }
                    },
                    new Game
                    {
                        Name = "Sid Meier's Civilization VI",
                        Description = "A turn-based 4X strategy game in which players build an empire to stand the test of time.",
                        ReleaseDate = new DateTime(2016, 10, 21),
                        CreatedAt = now,
                        Length = 30,
                        Developer = "Firaxis Games",
                        Slug = "civilization-vi",
                        BannerImage = "images/games/banners/civilization-vi-banner.png",
                        CoverImage = "images/games/covers/civilization-vi-cover.png",
                        Genres = new List<Genre> { strategy, simulation },
                        Platforms = new List<Platform> { pc, ps4, xboxOne, switchConsole, ios, android }
                    },
                    new Game
                    {
                        Name = "The Sims 4",
                        Description = "A life-simulation game where players create and control people, build homes, and shape their everyday lives.",
                        ReleaseDate = new DateTime(2014, 9, 2),
                        CreatedAt = now,
                        Length = 254,
                        Developer = "Maxis",
                        Slug = "the-sims-4",
                        BannerImage = "images/games/banners/the-sims-4-banner.png",
                        CoverImage = "images/games/covers/the-sims-4-cover.png",
                        Genres = new List<Genre> { simulation },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries, xboxOne }
                    },
                    new Game
                    {
                        Name = "EA Sports FC 24",
                        Description = "The latest installment in the world's most popular football simulation franchise.",
                        ReleaseDate = new DateTime(2023, 9, 29),
                        CreatedAt = now,
                        Length = 144,
                        Developer = "EA Sports",
                        Slug = "ea-sports-fc-24",
                        BannerImage = "images/games/banners/ea-sports-fc-24-banner.png",
                        CoverImage = "images/games/covers/ea-sports-fc-24-cover.png",
                        Genres = new List<Genre> { sports, simulation },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries, xboxOne, switchConsole }
                    },
                    new Game
                    {
                        Name = "Forza Horizon 5",
                        Description = "An open-world racing game set in a vibrant, ever-evolving recreation of Mexico.",
                        ReleaseDate = new DateTime(2021, 11, 9),
                        CreatedAt = now,
                        Length = 25,
                        Developer = "Playground Games",
                        Slug = "forza-horizon-5",
                        BannerImage = "images/games/banners/forza-horizon-5-banner.png",
                        CoverImage = "images/games/covers/forza-horizon-5-cover.png",
                        Genres = new List<Genre> { racing, sports },
                        Platforms = new List<Platform> { pc, xboxSeries, xboxOne }
                    },
                    new Game
                    {
                        Name = "Portal 2",
                        Description = "A first-person puzzle game built around a portal-creating gun, blending physics challenges with dark humor.",
                        ReleaseDate = new DateTime(2011, 4, 19),
                        CreatedAt = now,
                        Length = 10,
                        Developer = "Valve",
                        Slug = "portal-2",
                        BannerImage = "images/games/banners/portal-2-banner.png",
                        CoverImage = "images/games/covers/portal-2-cover.png",
                        Genres = new List<Genre> { puzzle, platformer },
                        Platforms = new List<Platform> { pc, ps4, xboxOne, switchConsole }
                    },
                    new Game
                    {
                        Name = "Resident Evil 4 Remake",
                        Description = "A reimagining of the survival-horror classic, following agent Leon Kennedy on a rescue mission in rural Europe.",
                        ReleaseDate = new DateTime(2023, 3, 24),
                        CreatedAt = now,
                        Length = 16,
                        Developer = "Capcom",
                        Slug = "resident-evil-4-remake",
                        BannerImage = "images/games/banners/resident-evil-4-remake-banner.png",
                        CoverImage = "images/games/covers/resident-evil-4-remake-cover.png",
                        Genres = new List<Genre> { horror, action, shooter },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries }
                    },
                    new Game
                    {
                        Name = "Street Fighter 6",
                        Description = "The latest entry in the legendary fighting-game series, featuring a deep roster and modern control schemes.",
                        ReleaseDate = new DateTime(2023, 6, 2),
                        CreatedAt = now,
                        Length = 69,
                        Developer = "Capcom",
                        Slug = "street-fighter-6",
                        BannerImage = "images/games/banners/street-fighter-6-banner.png",
                        CoverImage = "images/games/covers/street-fighter-6-cover.png",
                        Genres = new List<Genre> { fighting, action },
                        Platforms = new List<Platform> { pc, ps5, ps4, xboxSeries }
                    },
                    new Game
                    {
                        Name = "Half-Life: Alyx",
                        Description = "A VR-exclusive entry in the Half-Life series, set between the events of the first two games.",
                        ReleaseDate = new DateTime(2020, 3, 23),
                        CreatedAt = now,
                        Length = 12,
                        Developer = "Valve",
                        Slug = "half-life-alyx",
                        BannerImage = "images/games/banners/half-life-alyx-banner.png",
                        CoverImage = "images/games/covers/half-life-alyx-cover.png",
                        Genres = new List<Genre> { shooter, action, adventure },
                        Platforms = new List<Platform> { pc, quest, steamVR }
                    }
                );

                added = true;
            }

            try
            {
                if (added)
                {
                    // tornar persistentes os dados (necessário para os jogos terem Id antes das listas/reviews)
                    dbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            // ============== Utilizadores ==============
            if (!userManager.Users.Any())
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));

                // Conta de administrador
                var adminIdentity = new IdentityUser
                {
                    UserName = "BRKsEdu",
                    Email = "brksedu@achievement.com",
                    EmailConfirmed = true
                };

                // Criação do Admin e atribuição de role
                await userManager.CreateAsync(adminIdentity, "Administrador@123");
                await userManager.AddToRoleAsync(adminIdentity, "Admin");

                // Conta de utilizador normal
                var playerIdentity = new IdentityUser
                {
                    UserName = "DarkSoulsFan123",
                    Email = "darksouls@achievement.com",
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(playerIdentity, "DarkSouls123@");

                // Criação na tabela personalizada (Users)
                var admin = new User { 
                    Name = "BRKsEdu", 
                    Email = "brksedu@achievement.com",
                    Banner = "images/users/banners/admin-banner.png",
                    Image = "images/users/avatars/admin-avatar.png",
                    CreatedAt = now };
                var player = new User { 
                    Name = "DarkSoulsFan123", 
                    Email = "darksouls@achievement.com",
                    Banner = "images/users/banners/user-banner.png",
                    Image = "images/users/avatars/user-avatar.png",
                    CreatedAt = now };
                dbContext.Users.AddRange(admin, player);
                await dbContext.SaveChangesAsync(); // para obter os Id

                // Jogos a referenciar nas listas / reviews
                var witcher = await dbContext.Games.FirstAsync(g => g.Slug == "the-witcher-3-wild-hunt");
                var elden = await dbContext.Games.FirstAsync(g => g.Slug == "elden-ring");
                var darksouls = await dbContext.Games.FirstAsync(g => g.Slug == "dark-souls-iii");
                var zelda = await dbContext.Games.FirstAsync(g => g.Slug == "the-legend-of-zelda-breath-of-the-wild");
                var hollow = await dbContext.Games.FirstAsync(g => g.Slug == "hollow-knight");

                // Adicionar jogos às listas dos utilizadores
                dbContext.UserGames.AddRange(
                    new UserGame { UserFK = admin.Id, GameFK = witcher.Id, Status = GameStatus.Completed },
                    new UserGame { UserFK = admin.Id, GameFK = elden.Id, Status = GameStatus.Playing },
                    new UserGame { UserFK = player.Id, GameFK = darksouls.Id, Status = GameStatus.Completed },
                    new UserGame { UserFK = player.Id, GameFK = zelda.Id, Status = GameStatus.Completed },
                    new UserGame { UserFK = player.Id, GameFK = hollow.Id, Status = GameStatus.OnHold },
                    new UserGame { UserFK = player.Id, GameFK = elden.Id, Status = GameStatus.Completed }

                );

                // Reviews com nota
                dbContext.Reviews.AddRange(
                    new Review
                    {
                        UserFK = admin.Id,
                        GameFK = witcher.Id,
                        Rating = 9.5,
                        CreatedAt = now,
                        ReviewContent = "Todo jogador tem a obrigação de experimentar essa maravilhosa e inesquecível experiência de mundo aberto."
                    },
                    new Review
                    {
                        UserFK = admin.Id,
                        GameFK = elden.Id,
                        Rating = 9.0,
                        CreatedAt = now,
                        ReviewContent = "É um incrível jogo de RPG de ação e fantasia, com tudo o que se espera de um souls-like com um reino extenso que serve como convite ideal para horas de ação, lutas, sofrimento e recompensa."
                    },
                    new Review
                    {
                        UserFK = player.Id,
                        GameFK = zelda.Id,
                        Rating = 10.0,
                        CreatedAt = now,
                        ReviewContent = "Jogo legal, platinei em 1 dia"
                    },
                    new Review
                    {
                        UserFK = player.Id,
                        GameFK = darksouls.Id,
                        Rating = 10.0,
                        CreatedAt = now,
                        ReviewContent = "O MELHOR JOGO DO MUNDO NÃO TEM COMO!!!"
                    },
                    new Review
                    {
                        UserFK = player.Id,
                        GameFK = hollow.Id,
                        Rating = 8.5,
                        CreatedAt = now,
                        ReviewContent = "Ótimo indie, mas estou perdido no mapa faz um tempo :("
                    },
                    new Review
                    {
                        UserFK = player.Id,
                        GameFK = elden.Id,
                        Rating = 9.5,
                        CreatedAt = now,
                        ReviewContent = "MUITO BOM JOGO, mas Dark Souls III é melhor."
                    }
                );

                await dbContext.SaveChangesAsync();


                // Acerta a nota média dos jogos com review
                foreach (var gameId in new[] { witcher.Id, elden.Id, zelda.Id, hollow.Id })
                {
                    await dbContext.RecalculateGameRatingAsync(gameId);
                    await dbContext.RecalculateGamePlaysAsync(gameId);
                }
                await dbContext.SaveChangesAsync();

            }

        }
    }
}
