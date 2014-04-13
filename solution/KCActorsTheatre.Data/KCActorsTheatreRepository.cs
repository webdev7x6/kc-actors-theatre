using System.Data.Objects;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Data.Repositories;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreRepository : EntityFrameworkCmsRepository, IKCActorsTheatreRepository
    {
        private KCActorsTheatreDbContext context;
        private PostRepository postRepository = null;
        private AuthorRepository authorRepository = null;
        private CommentRepository commentRepository = null;
        private EventRepository eventRepository = null;
        private NewsletterSignUpRepository newsletterSignUpRepository = null;

        public KCActorsTheatreRepository(KCActorsTheatreDbContext entityFrameworkCmsDbContext)
            : base(entityFrameworkCmsDbContext)
        {
            this.context = entityFrameworkCmsDbContext;
        }

        public EventRepository Events
        {
            get
            {
                if (eventRepository == null)
                {
                    eventRepository = new EventRepository(context, this);
                }
                return eventRepository;
            }
        }

        public PostRepository Posts
        {
            get
            {
                if (postRepository == null)
                {
                    postRepository = new PostRepository(context, this);
                }
                return postRepository;
            }
        }

        public AuthorRepository Authors
        {
            get
            {
                if (authorRepository == null)
                {
                    authorRepository = new AuthorRepository(context, this);
                }
                return authorRepository;
            }
        }

        public CommentRepository Comments
        {
            get
            {
                if (commentRepository == null)
                {
                    commentRepository = new CommentRepository(context, this);
                }
                return commentRepository;
            }
        }

        public NewsletterSignUpRepository NewsletterSignUps
        {
            get
            {
                if (newsletterSignUpRepository == null)
                {
                    newsletterSignUpRepository = new NewsletterSignUpRepository(context, this);
                }
                return newsletterSignUpRepository;
            }
        }

        public override void RefreshAll()
        {
            base.RefreshAll();
            context.Refresh(RefreshMode.StoreWins, context.Events);
            context.Refresh(RefreshMode.StoreWins, context.Posts);
            context.Refresh(RefreshMode.StoreWins, context.Comments);
            context.Refresh(RefreshMode.StoreWins, context.Authors);
        }
    }
}