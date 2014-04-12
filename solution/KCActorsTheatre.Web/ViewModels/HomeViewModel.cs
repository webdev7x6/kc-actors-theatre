using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Cms.AppTypes;
using System.Collections.Generic;
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.MissionStories;

namespace KCActorsTheatre.Web.ViewModels
{
    public class HomeViewModel : KCActorsTheatreViewModel
    {
        public string BannerText { get; set; }
        public IEnumerable<RotatorImageContent> RotatorImageContents { get; set; }
        public ContentWidgetContent ContentWidgetContent { get; set; }
        public HomePageMissionStoryWidgetContent HomePageMissionStoryWidgetContent { get; set; }
        public IEnumerable<MissionStory> MissionStories { get; set; }
    }
}