using Clickfarm.AppFramework.Extensions;
using Clickfarm.AppFramework.Responses;
using Clickfarm.AppFramework.Web;
using Clickfarm.Cms.Core;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCActorsTheatre.Web.Controllers
{
    public class HomeController : KCActorsTheatreController
    {
        public HomeController(ICmsContext context, HttpContextBase httpContext) : base(context, httpContext) { }

        #region Views

        public ActionResult Index()
        {
            var vm = new HomeViewModel();
            InitializeViewModel(vm);
            SetHomeBannerText(vm);
            SetAnnouncementWidgetProperties(vm);
            SetResourceWidgetProperties(vm);
            SetHomeContentWidget(vm);
            SetHomeRotatorImages(vm);
            SetHomeMissionStories(vm);
            return View(vm);
        }

        public ActionResult Inner()
        {
            var vm = new InnerViewModel();
            InitializeViewModel(vm);

            SetHeroImageProperties(vm);
            SetResourceWidgetProperties(vm);
            SetAnnouncementWidgetProperties(vm);
            SetConnectWidgetContacts(vm);
            SetContentWidgets(vm);

            return View(vm);
        }

        public ActionResult Landing()
        {
            var vm = new LandingViewModel();
            InitializeViewModel(vm);

            SetHeroImageProperties(vm, CmsConfigConstants.ContentGroup_LandingPageHeroImage, CmsConfigConstants.ContentType_HeroImage);
            SetResourceWidgetProperties(vm);
            SetAnnouncementWidgetProperties(vm);
            SetConnectWidgetContacts(vm);
            SetContentWidgets(vm);

            return View(vm);
        }

        public ActionResult SiteMap()
        {
            var model = new HomeViewModel();
            InitializeViewModel(model);
            return View(model);
        }

        #endregion

        #region Private Helpers

        private void SetHomeContentWidget(HomeViewModel vm)
        {
            ContentGroup cg = null;
            ContentGroupMember cgm = null;

            cg = vm.RequestContent.Page != null ? vm.RequestContent.Page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_HomePageContentWidget) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_ContentWidget);

            if (cgm != null && cgm.Content != null)
            {
                vm.ContentWidgetContent = (ContentWidgetContent)cgm.Content;
            }
        }

        private void SetHomeBannerText(HomeViewModel vm)
        {
            ContentGroup cg = null;
            ContentGroupMember cgm = null;

            cg = vm.RequestContent.Page != null ? vm.RequestContent.Page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_BannerText) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_Text);

            if (cgm != null && cgm.Content != null)
            {
                var content = (TextContent)cgm.Content;
                vm.BannerText = content.Text;
            }
        }
        

        private void SetHomeMissionStories(HomeViewModel vm)
        {
            // get mission stories
            var repoResponse = repository.MissionStories.GetHomePageMissionStories();
            if (repoResponse.Succeeded)
            {
                vm.MissionStories = repoResponse.Entity.Take(4);
            }

            // get mission story widget content
            ContentGroup cg = null;
            ContentGroupMember cgm = null;

            cg = vm.RequestContent.Page != null ? vm.RequestContent.Page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_HomePageMissionStoriesWidget) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_MissionStoriesWidget);

            if (cgm != null && cgm.Content != null)
                vm.HomePageMissionStoryWidgetContent = (HomePageMissionStoryWidgetContent)cgm.Content;
        }

        private void SetHomeRotatorImages(HomeViewModel vm)
        {
            Page page = vm.RequestContent.Page;
            ContentGroup cg = null;
            IEnumerable<ContentGroupMember> cgMembers = null;

            cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_RotatorImages) : null;

            if (cg != null)
                cgMembers = cg.Members
                    .Where(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_RotatorImage)
                    ;

            var contents = new List<RotatorImageContent>();

            if (cgMembers != null && cgMembers.Count() > 0)
            {
                foreach (var cgMember in cgMembers)
                {
                    var rotatorImageContent = (RotatorImageContent)cgMember.Content;
                    if (rotatorImageContent != null)
                        contents.Add(rotatorImageContent);
                }

                if (contents.Count > 0)
                    vm.RotatorImageContents = contents
                        .OrderBy(p => p.RotatorOrder)
                        .ThenBy(p => p.RotatorTitle)
                        ;
            }
        }

        private void SetHeroImageProperties(KCActorsTheatreViewModel vm, string contentGroupName = CmsConfigConstants.ContentGroup_HeroImage, string contentMemberConfigName = CmsConfigConstants.ContentType_HeroImage)
        {
            ContentGroup cg = null;
            ContentGroupMember cgm = null;

            cg = vm.RequestContent.Page != null ? vm.RequestContent.Page.ContentGroups.FirstOrDefault(p => p.Name == contentGroupName) : null;
            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == contentMemberConfigName);
            if (cgm != null && cgm.Content != null)
            {
                // set different properties based on which type of hero image it is
                switch (contentGroupName)
                {
                    case CmsConfigConstants.ContentGroup_HeroImage:
                        var heroImageContent = (HeroImageContent)cgm.Content;
                        vm.HeroImageURL = heroImageContent.ImageUrl;
                        break;
                    case CmsConfigConstants.ContentGroup_LandingPageHeroImage:
                        var landingPageHeroImageContent = (LandingPageHeroImageContent)cgm.Content;
                        vm.HeroImageURL = landingPageHeroImageContent.ImageUrl;
                        vm.HeroImageTitle = landingPageHeroImageContent.HeroTitle;
                        vm.HeroImageCaption = landingPageHeroImageContent.HeroCaption;
                        break;
                }
            }
        }

        private void SetResourceWidgetProperties(KCActorsTheatreViewModel vm)
        {
            Page page = vm.RequestContent.Page;
            ContentGroup cg = null;
            ContentGroupMember cgm = null;
            ResourceWidgetContent resourceWidgetContent = null;

            cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_ResourcesWidget) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_ResourcesWidget);

            if (cgm != null && cgm.Content != null)
                resourceWidgetContent = (ResourceWidgetContent)cgm.Content;

            if (resourceWidgetContent != null)
            {
                // get tags and put into list
                var tagRepoResponse = repository.Tags.GetWidgetTags(resourceWidgetContent.ContentID);
                if (tagRepoResponse.Succeeded)
                {
                    // get resources and put into list
                    vm.ResourceWidgetTags = tagRepoResponse.Entity.Select(p => p.Name).ToList();
                    var resourseRepoResponse = repository.Resources.GetByTags(vm.ResourceWidgetTags);
                    if (resourseRepoResponse.Succeeded)
                        vm.ResourceWidgetResources = resourseRepoResponse.Entity;
                }
            }
        }

        private void SetConnectWidgetContacts(KCActorsTheatreViewModel vm)
        {
            Page page = vm.RequestContent.Page;
            ContentGroup cg = null;
            ContentGroupMember cgm = null;

            cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_ConnectWidget) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_ConnectWidget);

            if (cgm != null && cgm.Content != null)
            {
                var repoResponse = repository.StaffMembers.GetWidgetStaff(cgm.Content.ContentID);

                if (repoResponse.Succeeded)
                {
                    vm.ConnectWidgetContacts = repoResponse.Entity;
                }
            }
        }

        private void SetAnnouncementWidgetProperties(KCActorsTheatreViewModel vm)
        {
            Page page = vm.RequestContent.Page;
            ContentGroup cg = null;
            ContentGroupMember cgm = null;
            AnnouncementWidgetContent widgetContent = null;

            cg = page != null ? page.ContentGroups.FirstOrDefault(p => p.Name == CmsConfigConstants.ContentGroup_AnnouncementsWidget) : null;

            if (cg != null)
                cgm = cg.Members.FirstOrDefault(p => p.ContentGroupMemberConfigName == CmsConfigConstants.ContentType_AnnouncementsWidget);

            if (cgm != null && cgm.Content != null)
                widgetContent = (AnnouncementWidgetContent)cgm.Content;

            if (widgetContent != null)
            {
                // get tags and put into list
                var tagRepoResponse = repository.Tags.GetWidgetTags(widgetContent.ContentID);
                if (tagRepoResponse.Succeeded)
                {
                    // get announcments and put into list
                    vm.AnnouncementWidgetTags = tagRepoResponse.Entity.Select(p => p.Name).ToList();
                    var announcementRepoResponse = repository.Announcements.GetByTags(vm.AnnouncementWidgetTags);
                    if (announcementRepoResponse.Succeeded)
                        vm.AnnouncementWidgetAnnouncements = announcementRepoResponse.Entity;
                }
            }
        }

        #endregion

    }
}