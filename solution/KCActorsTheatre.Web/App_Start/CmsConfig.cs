using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Clickfarm.AppFramework.Logging;
using Clickfarm.Cms.Configuration;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Data;
using Clickfarm.Cms.Data.Repositories;
using Clickfarm.Cms.Init.Autofac.Modules;
using Clickfarm.Cms.Membership;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Cms;
using KCActorsTheatre.Data;
using KCActorsTheatre.Cms.ContentTypes;

namespace KCActorsTheatre.Web
{
    public static class CmsConfig
    {
        public static void Bootstrap()
        {
            ModelMetadataProviders.Current = new CmsModelMetadataProvider();

            #region Autofac registration
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterFilterProvider();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterControllers(Assembly.GetAssembly(typeof(Clickfarm.Cms.Admin.Mvc.Areas.Admin.Controllers.HomeController)));
            builder.RegisterModule(new AdminModule
            {
                TemplateAssemblyFiles = new string[] { HttpContext.Current.Server.MapPath("~/Bin/KCActorsTheatre.Web.dll") }
            });
            builder.RegisterModule(new EntityFrameworkModule());
            builder.RegisterType<KCActorsTheatreRepository>().As<ICmsRepository>();
            builder.RegisterType<KCActorsTheatreDbContext>()
                .AsSelf()
                .As<ICmsDbContext>()
                .WithParameter((p, cx) => p.Name == "nameOrConnectionString", (p, cx) => KCActorsTheatreDbContext.GetConnStrNameToUse())
            ;

            builder.RegisterType<MembershipService>().As<IMembershipService>()
                .WithProperty("SessionCookieKey", "kj7c5ycxvz6s")
                .WithProperty("IDValueCookieKey", "h8ipqkesjx6w")
                .WithProperty("CookieExpires", 365)
            ;

            builder.RegisterType<Log4NetLogger>().As<ILogger>()
                .WithProperty("ValidMachines", new[]
                {
                    Constants.MachineName_DevWeb,
                    //Constants.MachineName_ProdWeb,
                    //Constants.MachineName_IqmWeb01
                })
                .WithProperty("IgnoreExceptionTypes", new[]
                {
                    typeof(HttpException)
                })
            ;

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
            #endregion

            var configBuilder = DependencyResolver.Current.GetService<CmsConfigBuilder>();

            #region Global
            //global
            configBuilder.Global()
                .HasProjectName("KC Actors Theatre CMS")
                .HasMenuItems(new AdminMenuItem[]
                {
                    new AdminMenuItem
                    {
                        Title = "News",
                        Href = "/CustomAdmin/News",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator,
                            Constants.CmsRole_CalendarManager,
                        }
                    }
                })
            ;

            //page types
            configBuilder.PageType<WebPage>("Web Page");

            #endregion

            #region Machines

            configBuilder.DefaultMachine()
                .UsesCdnHost("clickfarmcdn.localhost")
                //.UsesConnectionString("Development")
                .UsesConnectionString("Workstation")
            ;
            configBuilder.Machine(Constants.MachineName_DevWeb)
                .UsesLocalCdnPath()
            ;
            configBuilder.Machine(Constants.MachineName_ProdWeb)
                .UsesCdnHost("clickfarmcdn.com")
                .IsProduction()
            ;

            #endregion

            #region Content types

            configBuilder.Content<CalloutTextContent>("Callout Text")
                .UsesView("TextContent")
                .HasIconCssClass("ui-icon-comment")
                .WithProperties(new TextContentProperties
                {
                    CharacterLimit = 225
                })
            ;

            configBuilder.Content<HtmlContent>("HTML")
                .HasIconCssClass("ui-icon-carat-2-e-w")
                .WithProperties("FileImageContentProperties", new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Images",
                    DefaultSubfolder = "Content",
                    MediaTypes = new string[] { "image/" }
                })
                .WithProperties("FileDocumentContentProperties", new FileContentProperties
                {
                    RootFolder = "/Common/Cms/Documents"
                })
            ;

            configBuilder.Content<HeroImageContent>("Hero Image")
                .UsesView("ImageContent")
                .HasIconCssClass("ui-icon-image")
                .HasDataEntryHint("Hero images are 870px wide with a max height of 400px.")
                .WithProperties(new FileContentProperties
                {

                    RootFolder = "/common/cms/images/hero",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 870,
                    MaxHeight = 400
                })
            ;

            configBuilder.Content<RotatorImageContent>("Rotator Image")
                .HasIconCssClass("ui-icon-image")
                .HasDataEntryHint("Rotator images are 770 x 540.")
                .WithProperties("FileDocumentContentProperties", new FileContentProperties
                {
                    RootFolder = "/common/cms/files",
                    DefaultSubfolder = ""
                })
                .WithProperties("FileImageContentProperties", new FileContentProperties
                {
                    RootFolder = "/common/cms/images/rotator",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 770,
                    ExactHeight = 540
                })
            ;

            configBuilder.Content<RotatorVideoContent>("Rotator Video").HasIconCssClass("ui-icon-video");

            configBuilder.Content<TextContent>("Get Involved").HasIconCssClass("ui-icon-script");
            configBuilder.Content<TextContent>("Upcoming Events").HasIconCssClass("ui-icon-script");
            configBuilder.Content<TextContent>("About Healthy Nevada").HasIconCssClass("ui-icon-script");


            #endregion

            #region Content groups

            configBuilder.ContentGroup("Body Copy").AsFixed().HasContentType<HtmlContent>("Copy");
            configBuilder.ContentGroup("Home Callouts")
                .AsFixed()
                .HasContentType<TextContent>("Get Involved")
                .HasContentType<TextContent>("Upcoming Events")
                .HasContentType<TextContent>("About Healthy Nevada")
            ;

            configBuilder.ContentGroup("Hero")
                .AsFixed()
                .HasContentType<HeroImageContent>("Hero Image")
            ;

            configBuilder.ContentGroup("Rotator Slides")
                .HasContentType<RotatorImageContent>("Rotator Image")
                .HasContentType<RotatorVideoContent>("Rotator Video")
            ;

            #endregion

            #region Apps

            #region KCActorsTheatre Website

            AppConfiguration app = configBuilder.App("KC Actors Theatre Website")
                .HasPageType("Web Page")
                .UsesEditView("_KCActorsTheatreAppEdit")
                ;

            app.OnDefaultMachine()
                .HasHost<PreviewRequestContentHandler>("localhost", "Live")
                ;

            app.OnMachine(Constants.MachineName_DevWeb)
                .HasHost<PreviewRequestContentHandler>("KCActorsTheatre.clickfarminteractive.com", "Live")
                ;

            app.OnMachine(Constants.MachineName_ProdWeb)
                .HasHost<CachedRequestContentHandler>("www.KCActorsTheatre.net", "Live", isPrimary:true)
                .HasHost<CachedRequestContentHandler>("67.59.163.21", "IP")
                ;

            app.HasController<Controllers.HomeController>("Site Map", "SiteMap")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Commmunity Dashboard", "CommunityDashboard")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Home Page", "Index")
                .ForPageType("Web Page")
                .HasContentGroup("Home Callouts")
                .HasContentGroup("Rotator Slides")
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("News Landing Page", "Index")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("News Detail Page", "Detail")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Inner Page", "Inner")
                .IsDefault()
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                ;

            app.HasController<Controllers.SearchController>("Search", "Index")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;


            #endregion

            #endregion
        }
    }
}
