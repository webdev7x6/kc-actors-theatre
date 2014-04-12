using Clickfarm.Cms.Configuration;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Clickfarm.AppFramework.Extensions;
using KCActorsTheatre.Cms.ContentTypes;

namespace KCActorsTheatre.Cms.Core
{
    public class CustomCachedRequestContentHandler : CachedRequestContentHandler
    {
        private List<App> apps;
		private List<Content> cmsKeyedContent;
		private List<AssignedSchedule> assignedSchedules;

		public CustomCachedRequestContentHandler(IScheduleService scheduleService) : base(scheduleService)
		{
			Init();
		}

        public CustomCachedRequestContentHandler()
        {
			Init();
		}

		private void Init()
		{
			var repository = DependencyResolver.Current.GetService<ICmsRepository>();
			apps = repository.Apps.GetAppsForRequestContent();
			InitializeAssignedSchedules(repository);
			InitializeCmsKeyedContent(repository);
		}

		public override RequestContent GetRequestContent(HttpContextBase httpContext, string hostName, string urlPath, DateTime now)
        {
            this.RequireParameter(httpContext, "httpContext");
            this.RequireParameter(hostName, "hostName");
            this.RequireParameter(urlPath, "urlPath");

            AppConfigData thisApp = null;
            AppHostConfigData thisHost = null;
            GetThisAppAndHost(hostName, out thisApp, out thisHost);
            if (thisApp != null && thisHost != null)
            {
                App app = apps.FirstOrDefault(a => a.Name.Equals(thisApp.AppName, StringComparison.CurrentCultureIgnoreCase));
                if (app != null)
                {
                    Url url = app.Urls.FirstOrDefault(u => u.Path.Equals(urlPath, StringComparison.CurrentCultureIgnoreCase));
					if (url != null)
					{
						if (url.AssignedSchedule == null)
						{
							url.AssignedSchedule = assignedSchedules.Where(s => s.Url != null && s.Url.UrlID == url.UrlID).FirstOrDefault();
						}
						if (url.Page.AssignedSchedule == null)
						{
							url.Page.AssignedSchedule = assignedSchedules.Where(s => s.Page != null && s.Page.PageID == url.Page.PageID).FirstOrDefault();
						}

						return GetRequestContent(url, url.Page, now, () =>
						{
							RequestContent content = new RequestContent { Url = url, Page = url.Page, App = app, Request = httpContext.Request };

                            //// ConnectWidgetContent
                            //ContentGroup cg = content.Page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_ConnectWidget);

                            //if (cg!= null)
                            //{
                            //    ContentGroupMember cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_ResourcesWidget);

                            //    if (cgm != null)
                            //    {
                            //        ConnectWidgetContent cwc = (ConnectWidgetContent)cgm.Content;

                            //        // check if StaffMembers collection is null and populate if needed
                            //        if (cwc.StaffMembers == null)
                            //        {
                            //            //cwc.StaffMembers = 
                            //        }
                            //    }
                            //}

							content.Menus.AddRange(app.Menus);

							content.GetContentForCmsKey = (cmsKey) =>
							{
								Content matchingContent = cmsKeyedContent.Where(c => c.CmsKey == cmsKey && c.Enabled).FirstOrDefault();
								if (matchingContent != null)
								{
									return new[] { new GlobalContentGroupMember(matchingContent) };
								}
								else
								{
									return null;
								}
							};
							return content;
						});
					}
                    //2012-10-24 BA - I'm not sure we should do this
                    return GetRequestContentFromNextHandler(httpContext, hostName, urlPath, now);
                }
                //2012-10-24 BA - I'm not sure we should do this
                return GetRequestContentFromNextHandler(httpContext, hostName, urlPath, now);
            }
            return GetRequestContentFromNextHandler(httpContext, hostName, urlPath, now);
        }

		public override void Refresh(App app)
        {
            this.RequireParameter(app, "app");

            if (IsValidApp(app))
            {
                var repository = DependencyResolver.Current.GetService<ICmsRepository>();
                App appToRefresh = apps.Single(a => a.AppID == app.AppID);
                int appID = appToRefresh.AppID;
                apps.Remove(appToRefresh);
                appToRefresh = null;
                App newApp = repository.Apps.GetAppForRequestContent(appID);
                if (newApp != null)
                {
                    apps.Add(newApp);
                }
				InitializeCmsKeyedContent(repository);
				InitializeAssignedSchedules(repository);
            }
            RefreshNextHandler(app);
        }

		private void InitializeCmsKeyedContent(ICmsRepository repository)
		{
			cmsKeyedContent = repository.Content.GetAllContentWithCmsKey();
		}

		private void InitializeAssignedSchedules(ICmsRepository repository)
		{
			assignedSchedules = repository.AssignedSchedules.GetAssignedSchedulesForRequestContent();
		}
    }
}
