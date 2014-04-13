using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using KCActorsTheatre.Contract;

namespace KCActorsTheatre.Data.Repositories
{
    public class PersonRepository : KCActorsTheatreRepositoryBase<Person>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public PersonRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<Person> DbSet
        {
            get { return dbContext.People; }
        }

        public RepositoryResponse<Person> New(Person item)
        {
            return CatchError<RepositoryResponse<Person>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<Person>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Person>> FindForDisplay(IEnumerable<string> terms)
        {
            return CatchError<RepositoryResponse<IEnumerable<Person>>>(() =>
            {
                var items = All(null, enableTracking: false);
                if (terms != null && terms.Count() > 0)
                {
                    items = items.Where(a =>
                        terms.Any(t =>
                            a.Name != null && a.Name.IndexOf(t, StringComparison.CurrentCultureIgnoreCase) >= 0
                        )
                    );
                }
                var response = new RepositoryResponse<IEnumerable<Person>>();
                response.Succeed(string.Format("{0} items(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Person>> Find(IEnumerable<string> terms)
        {
            var resp = new RepositoryResponse<IEnumerable<Person>>();
            try
            {
                var people = this.All();
                resp.Entity = people
                    .Where(c => terms.Any(t => c.Name.ToLower().IndexOf(t.ToLower()) > -1))
                    .OrderBy(c => c.Name)
                    .ToList()
                ;
                resp.Succeed(string.Format("{0} {1} found.", resp.Entity.Count(), "People"));
            }
            catch (Exception ex)
            {
                resp.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return resp;
        }


        public RepositoryResponse<IEnumerable<Person>> FindForWebsite(string searchTerm)
        {
            return CatchError<RepositoryResponse<IEnumerable<Person>>>(() =>
            {
                var items = All(null, enableTracking: false);
                if (searchTerm.Length > 0)
                {
                    items = items.Where(p => p.Title.IndexOf(searchTerm, StringComparison.CurrentCultureIgnoreCase) >= 0);
                }
                var response = new RepositoryResponse<IEnumerable<Person>>();
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.Name).ThenBy(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Person>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<Person>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<Person>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderByDescending(a => a.DateCreated);
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<Person>> GetForWebsite(int? howMany = null, int? skip = null)
        {
            return CatchError<RepositoryResponse<IEnumerable<Person>>>(() =>
            {
                var people = All()
                    .OrderBy(p => p.Name)
                    .ThenBy(p => p.DateCreated)
                    .ToList()
                ;
                if (skip.HasValue)
                    people = people.Skip(skip.Value).ToList();
                if (howMany.HasValue)
                    people = people.Take(howMany.Value).ToList();
                var response = new RepositoryResponse<IEnumerable<Person>>();
                response.Succeed(string.Format("{0} item(s) found.", people.Count()));
                response.Entity = people;
                return response;
            });
        }


        public RepositoryResponse<Person> GetSingle(int id)
        {
            return CatchError<RepositoryResponse<Person>>(() =>
            {
                var item = Single(a => a.PersonID == id, null, enableTracking:true);
                var response = new RepositoryResponse<Person>();
                if (item != null)
                {
                    response.Succeed(string.Format("Item with ID {0} found.", item.PersonID));
                    response.Entity = item;
                }
                else
                {
                    response.Fail(string.Format("Item with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse Delete(int id)
        {
            return CatchError<RepositoryResponse>(() =>
            {
                var item = Find(id);
                var response = new RepositoryResponse();
                if (item != null)
                {
                    CmsDbContext.ChangeState<Person>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("Person with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("Person with ID {0} not found.", id));
                }
                return response;
            });
        }
    }
}