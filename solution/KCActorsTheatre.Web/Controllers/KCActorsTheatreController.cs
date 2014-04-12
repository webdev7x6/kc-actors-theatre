using System;
using System.Web.Mvc;
using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Logging;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Cms;

namespace KCActorsTheatre.Web.Controllers
{
    public class KCActorsTheatreController : CmsController
    {
        protected ICmsContext context { get; private set; }
        protected IKCActorsTheatreRepository repository { get; private set; }
        private HttpSessionStateBase session = null;

        public KCActorsTheatreController(ICmsContext context, HttpContextBase httpContext)
        {
            this.context = this.context;
            this.repository = context.Repository as IKCActorsTheatreRepository;
            this.session = httpContext.Session;
        }

        protected void InitializeViewModel(KCActorsTheatreViewModel model)
        {
            model.RequestContent = this.CmsRequestContent;
        }

        protected void SetContentWidgets(KCActorsTheatreViewModel vm)
        {
            Page page = vm.RequestContent.Page;
            ContentGroup cg = null;
            IEnumerable<ContentGroupMember> cgMembers = null;

            cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_ContentWidgets) : null;

            if (cg != null)
                cgMembers = cg.Members
                    .Where(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_ContentWidget)
                    ;

            var contentWidgets = new List<ContentWidgetContent>();

            if (cgMembers != null && cgMembers.Count() > 0)
            {
                foreach (var cgMember in cgMembers)
                {
                    if (cgMember.Content != null)
                    {
                        var connectWidgetContent = (ContentWidgetContent)cgMember.Content;
                        if (connectWidgetContent != null)
                            contentWidgets.Add(connectWidgetContent);
                    }
                }

                if (contentWidgets.Count > 0)
                    vm.ContentWidgets = contentWidgets
                        .OrderBy(p => p.DisplayOrder)
                        .ThenBy(p => p.Title)
                        ;
            }
        }

        private JsonResult CatchError(Func<JsonResult> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                DependencyResolver.Current.GetService<ILogger>().LogWeb(HttpContext, LogLevel.Error, "A JSON response exception occurred on KCActorsTheatre.com.", ex.GetInnermostException());
                return new JsonResult
                {
                    Data = new ErrorViewModel(CmsRequestContent, ex),
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}