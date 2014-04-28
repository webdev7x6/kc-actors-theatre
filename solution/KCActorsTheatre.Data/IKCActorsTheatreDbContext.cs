using System.Data.Entity;
using Clickfarm.Cms.Data;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreDbContext : ICmsDbContext
    {
        DbSet<Article> NewsArticles { get; }

        DbSet<ShowInfo> Shows { get; }
        DbSet<Person> People { get; }
        DbSet<ShowImage> Images { get; }
        DbSet<ShowVideo> Videos { get; }

        DbSet<SeasonInfo> Seasons { get; }
    }
}