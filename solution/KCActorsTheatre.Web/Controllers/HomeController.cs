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
using KCActorsTheatre.Web.Helpers;
using System.Text;

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

        public ActionResult InnerHero()
        {
            var model = new KCActorsTheatreViewModel();
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
            //var app = (KCActorsTheatre.Library.AppTypes.KCActorsTheatreApp)model.RequestContent.App;

            var message = new StringBuilder("The following was submitted on the Contact Us page of tht KCAT website.<br /><br />");
            message.AppendFormat("Name: {0}<br />", model.Name);
            message.AppendFormat("Email: {0}<br />", model.Email);
            message.AppendFormat("Phone Number: {0}<br />", model.PhoneNumber);

            if (!string.IsNullOrEmpty(model.MailingAddress))
                message.AppendFormat("Mailing Address: {0}<br />", model.MailingAddress);

            message.AppendFormat("Comments: {0}", model.Comments);


            var email = new Emailer();
            email.To = "pete.aaron.davis@gmail.com"; // app.ContactUsEmail; ;
            email.Subject = "KCAT Website Contact Us Submission";
            email.Message = message.ToString();
            email.Send();
            

            return Contact();
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