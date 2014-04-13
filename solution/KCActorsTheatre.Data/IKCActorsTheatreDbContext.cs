using System.Data.Entity;
using Clickfarm.Cms.Data;
using KCActorsTheatre.Blog;
using KCActorsTheatre.Calendar;

namespace KCActorsTheatre.Data
{
    public interface IKCActorsTheatreDbContext : ICmsDbContext
    {
        DbSet<Event> Events { get; }
        DbSet<Author> Authors { get; }
        DbSet<Post> Posts { get; }
        DbSet<Comment> Comments { get; }
        DbSet<NewsletterSignUp> NewsletterSignUps { get; }
    }
}
