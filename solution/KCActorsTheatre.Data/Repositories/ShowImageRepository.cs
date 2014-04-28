using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCActorsTheatre.Data.Repositories;
using KCActorsTheatre.Contract;
using KCActorsTheatre.Data;

namespace KCActorsTheatre.Data.Repositories
{
    public class ShowImageRepository : KCActorsTheatreRepositoryBase<ShowImage>
    {
        private IKCActorsTheatreRepository _repository;
        private IKCActorsTheatreDbContext _context;
        public ShowImageRepository(IKCActorsTheatreDbContext context, IKCActorsTheatreRepository repository)
            : base(context, repository)
        {
            _context = context;
            _repository = repository;
        }

        protected override DbSet<ShowImage> DbSet
        {
            get { return dbContext.Images; }
        }

        public RepositoryResponse<ShowImage> New(ShowImage item)
        {
            return CatchError<RepositoryResponse<ShowImage>>(() =>
            {
                DbSet.Add(item);
                CmsDbContext.Save();
                var response = new RepositoryResponse<ShowImage>();
                response.Succeed("The new item was created successfully.");
                response.Entity = item;
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
                    CmsDbContext.ChangeState<ShowImage>(EntityState.Deleted, item);
                    CmsDbContext.Save();
                    response.Succeed(string.Format("ShowImage with ID {0} deleted.", id));
                }
                else
                {
                    response.Fail(string.Format("ShowImage with ID {0} not found.", id));
                }
                return response;
            });
        }

        public RepositoryResponse<IEnumerable<ShowImage>> GetAll()
        {
            return CatchError<RepositoryResponse<IEnumerable<ShowImage>>>(() =>
            {
                var response = new RepositoryResponse<IEnumerable<ShowImage>>();
                var items = All(null, enableTracking: false);
                response.Succeed(string.Format("{0} item(s) found.", items.Count()));
                response.Entity = items.OrderBy(a => a.DisplayOrder);
                return response;
            });
        }

        public RepositoryResponse UpdateImageDisplayOrder(int[] showImageIDs)
        {
            var response = new RepositoryResponse();

            try
            {
                for (int i = 0; i < showImageIDs.Length; i++)
                {
                    var id = showImageIDs[i];
                    var showImage = this.Single(p => p.ShowImageID == id);
                    showImage.DisplayOrder = i;
                }

                CmsDbContext.Save();

                response.Succeed("display order successfully sorted");
            }
            catch (Exception ex)
            {
                response.Fail(string.Format("An exception occurred: {0}", ex.GetInnermostException().Message));
            }
            return response;
        }
    }
}