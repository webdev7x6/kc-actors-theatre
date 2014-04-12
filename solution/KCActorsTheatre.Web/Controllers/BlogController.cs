using Clickfarm.AppFramework.Logging;
using Clickfarm.AppFramework.Responses;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Blogs;
using KCActorsTheatre.Services.Mapping;
using KCActorsTheatre.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class BlogController : KCActorsTheatreController
    {
        readonly IMappingService _mapper;
        private IJsonMapper JsonMapper { get; set; }

        public BlogController(ICmsContext context, HttpContextBase httpContext, IMappingService mapper, IJsonMapper jsonMapper) : base(context, httpContext) 
        {
            _mapper = mapper;
            JsonMapper = jsonMapper;
        }

        public ActionResult DailyBread()
        {
            var vm = new BlogsViewModel();
            vm.BlogType = BlogType.DailyBread;
            vm.Categories = repository.PostCategories.All();
            vm.ArchiveData = GetArchiveData(vm.BlogType);
            vm.DisqusAccount = "daily-bread";
            InitializeViewModel(vm);
            SetContentWidgets(vm);
            return View("Blog", vm);
        }

        public ActionResult Evangelist()
        {
            var vm = new BlogsViewModel();
            vm.BlogType = BlogType.Evangelist;
            vm.Categories = repository.PostCategories.All();
            vm.ArchiveData = GetArchiveData(vm.BlogType);
            vm.DisqusAccount = "evangelist";
            InitializeViewModel(vm);
            SetContentWidgets(vm);
            return View("Blog", vm);
        }

        [HttpPost]
        public JsonResult GetPost(int? postID)
        {
            if (!postID.HasValue || postID.Value <= 0)
                return Json(JsonMapper.Failed("The post id was invalid."));

            var repoResponse = repository.Posts.GetPostForSite(postID.Value);

            if (repoResponse.Succeeded)
                return Json(JsonMapper.Post(repoResponse.Entity));

            return Json(JsonMapper.Failed(repoResponse.Message));
        }

        [HttpPost]
        public JsonResult GetPosts(BlogType? blogType, int skip, int categoryID, int? month = null, int? year = null)
        {
            if (!blogType.HasValue)
                return Json(JsonMapper.Failed("The blog type was invalid."));

            var repoResponse = repository.Posts.GetPostsForSite(blogType.Value, categoryID, month, year);

            if (repoResponse.Succeeded)
            {
                var posts = repoResponse.Entity;

                var returnObj = new
                {
                    posts = JsonMapper.Posts(repoResponse.Entity.Skip(skip).Take(5)),
                    totalCount = repoResponse.Entity.Count(),
                };
                return Json(returnObj);
            }

            return Json(JsonMapper.Failed(repoResponse.Message));
        }

        private Dictionary<int, Dictionary<int, int>> GetArchiveData(BlogType blogType)
        {
            var dict = new Dictionary<int, Dictionary<int, int>>();

            var repoResponse = repository.Posts.GetPostsForSite(blogType, categoryID: 0);

            if (repoResponse.Succeeded)
            {
                repoResponse.Entity
                    .GroupBy(p => p.DateToPost.Value.Year)
                    .ToList()
                    .ForEach((year) =>
                    {
                        var months = new Dictionary<int, int>();
                        year
                            .GroupBy(p => p.DateToPost.Value.Month)
                            .ToList()
                            .ForEach((month) =>
                            {
                                months.Add(month.Key, month.Count());
                            });

                        dict.Add(year.Key, months);
                    });
            }

            return dict;
        }
    }
}