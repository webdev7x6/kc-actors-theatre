using Clickfarm.Cms.Data.Repositories;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreRepository : ICmsRepository
    {
        NewsArticleRepository NewsArticles { get; }
        ShowRepository Shows { get; }
        PersonRepository People { get; }
    }
}