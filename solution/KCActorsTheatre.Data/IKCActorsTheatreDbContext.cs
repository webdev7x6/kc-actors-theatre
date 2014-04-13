using System.Data.Entity;
using Clickfarm.Cms.Data;
using KCActorsTheatre.News;
using KCActorsTheatre.Show;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreDbContext : ICmsDbContext
    {
        DbSet<Article> NewsArticles { get; }
        DbSet<ShowInfo> Shows { get; }
    }
}