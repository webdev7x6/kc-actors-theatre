using Clickfarm.Cms.Data.Repositories;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreRepository : ICmsRepository
    {
        EventRepository Events { get; }
        PostRepository Posts { get; }
        AuthorRepository Authors { get; }
        CommentRepository Comments { get; }
        NewsletterSignUpRepository NewsletterSignUps { get; }
    }
}
