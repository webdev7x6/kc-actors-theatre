﻿using System.Reflection;
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
                    Constants.MachineName_Dev
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
                            Constants.CmsRole_SystemAdministrator
                        }
                    },
                     new AdminMenuItem
                    {
                        Title = "Shows",
                        Href = "/CustomAdmin/Show",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator
                        }
                    },
                    new AdminMenuItem
                    {
                        Title = "People",
                        Href = "/CustomAdmin/Person",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator
                        }
                    },
                    new AdminMenuItem
                    {
                        Title = "Seasons",
                        Href = "/CustomAdmin/Season",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator
                        }
                    }
                })
            ;

            //page types
            configBuilder.PageType<WebPage>("Web Page");

            #endregion

            #region Machines

            configBuilder.DefaultMachine()
                .UsesLocalCdnPath("common/Cdn")
                .UsesConnectionString("Workstation")
                //.UsesConnectionString(Constants.MachineName_Prod)
                .IsProduction();
            ;

            configBuilder.Machine(Constants.MachineName_Dev)
                .UsesLocalCdnPath("common/Cdn")
                .UsesConnectionString(Constants.MachineName_Dev)
            ;

            //configBuilder.DefaultMachine()
            //    .UsesLocalCdnPath("common/Cdn")
            //    .UsesConnectionString("Workstation")
            //;
            //configBuilder.Machine(Constants.MachineName_Prod)
            //    .UsesLocalCdnPath("common/Cdn")
            //    .IsProduction()
            //;

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
                .HasDataEntryHint("Hero images are 1200px wide with a height of 500px.")
                .WithProperties(new FileContentProperties
                {

                    RootFolder = "/common/cms/images/hero",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 1200,
                    MaxHeight = 500
                })
            ;

            configBuilder.Content<ImageContent>("Rotator Image")
                .UsesView("ImageContent")
                .HasIconCssClass("ui-icon-image")
                .HasDataEntryHint("Rotator images are 1200 x 1000.")
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
                    ExactWidth = 1200,
                    ExactHeight = 1000
                })
            ;

            configBuilder.Content<TextContent>("Get Involved").HasIconCssClass("ui-icon-script");
            configBuilder.Content<TextContent>("Upcoming Events").HasIconCssClass("ui-icon-script");
            configBuilder.Content<TextContent>("About Healthy Nevada").HasIconCssClass("ui-icon-script");


            #endregion

            #region Content groups

            configBuilder.ContentGroup("Body Copy").AsFixed().HasContentType<HtmlContent>("Copy");

            configBuilder.ContentGroup("Home Callout")
                .AsFixed()
                .HasContentType<TextContent>("Title")
                .HasContentType<TextContent>("Text")
            ;

            configBuilder.ContentGroup("Hero")
                .AsFixed()
                .HasContentType<HeroImageContent>("Hero Image")
            ;

            configBuilder.ContentGroup("Rotator Images")
                .HasContentType<ImageContent>("Rotator Image")
            ;

            #endregion

            #region Apps

            #region KCActorsTheatre Website

            AppConfiguration app = configBuilder.App("KC Actors Theatre Website")
                .HasPageType("Web Page")
                .UsesEditView("_KCActorsTheatreAppEdit")
                ;

            // DEV
            app.OnMachine(Constants.MachineName_Dev)
                .HasHost<PreviewRequestContentHandler>("kcat.agathoskc.com", "Live", isPrimary: true)
                .HasHost<CachedRequestContentHandler>("216.243.153.209", "IP")
                ;

            // PRODUCTION
            app.OnDefaultMachine()
                .HasHost<PreviewRequestContentHandler>("localhost", "Local")
                .HasHost<CachedRequestContentHandler>("www.kcactors.org", "Live", isPrimary: true)
                ;

            app.HasController<Controllers.HomeController>("Site Map", "SiteMap")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Contact Form", "Contact")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Home Page", "Index")
                .ForPageType("Web Page")
                .HasContentGroup("Home Callout")
                .HasContentGroup("Rotator Images")
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("News Landing Page", "Index")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("News Detail Page", "Article")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.PeopleController>("People Landing Page", "Index")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                ;

            app.HasController<Controllers.PeopleController>("Founders Page", "Founders")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;

            app.HasController<Controllers.PeopleController>("Board Page", "Board")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;

            app.HasController<Controllers.PeopleController>("Artists Page", "Artists")
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                .SingleUse()
                ;

            app.HasController<Controllers.PeopleController>("Person Detail Page", "Person")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.ShowController>("Show Detail Page", "Item")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.SeasonController>("Season Detail Page", "Item")
                .ForPageType("Web Page")
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Inner Page", "Inner")
                .IsDefault()
                .ForPageType("Web Page")
                .HasContentGroup("Body Copy")
                ;

            app.HasController<Controllers.HomeController>("Inner Page With Hero", "InnerHero")
                .ForPageType("Web Page")
                .HasContentGroup("Hero")
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
