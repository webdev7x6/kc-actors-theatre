using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class HomeController : KCActorsTheatreController
    {
        public HomeController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) { }

        public ActionResult Index()
        {
            var model = new HomeViewModel();
            InitializeViewModel(model);
            SetRotatorImages(model);
            return View(model);
        }

        public ActionResult Inner()
        {
            var model = new InnerViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        public ActionResult SiteMap()
        {
            var model = new HomeViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        [HttpGet]
        public ActionResult Contact()
        {
            var model = new ContactViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Contact(ContactViewModel model)
        {


            return Contact();
        }

        public ActionResult CommunityDashboard()
        {
            var model = new InnerViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        private void SetRotatorImages(HomeViewModel vm)
        {
            if(vm.RequestContent != null)
            {
                Page page = vm.RequestContent.Page;
                ContentGroup cg = null;
                IEnumerable<ContentGroupMember> cgMembers = null;

                cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == "Rotator Images") : null;

                if (cg != null)
                    cgMembers = cg.Members
                        .Where(p => p.ContentGroupMemberConfigName == "Rotator Image")
                        ;

                var contentCollection = new List<ImageContent>();

                if (cgMembers != null && cgMembers.Count() > 0)
                {
                    foreach (var cgMember in cgMembers)
                    {
                        if (cgMember.Content != null)
                        {
                            var content = (ImageContent)cgMember.Content;
                            if (content != null)
                                contentCollection.Add(content);
                        }
                    }

                    if (contentCollection.Count > 0)
                        vm.RotatorImages = contentCollection
                            ;
                }
            }            
        }
    }
}