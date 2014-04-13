using System.Data.Entity;
using Clickfarm.Cms.Data;
using KCActorsTheatre.News;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreDbContext : ICmsDbContext
    {
        DbSet<Article> NewsArticles { get; }
    }
}