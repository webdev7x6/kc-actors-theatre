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
using KCActorsTheatre.Cms.ContentTypes;
using KCActorsTheatre.Cms.Core;
using KCActorsTheatre.Data;
using KCActorsTheatre.Services.Mapping;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

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

            builder.RegisterType<AutoMapperService>().As<IMappingService>();
            builder.RegisterType<JsonMapper>().As<IJsonMapper>();

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

            builder.RegisterType<CustomCachedRequestContentHandler>().AsSelf().UsingConstructor(typeof(IScheduleService)).SingleInstance();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));

            #endregion

            var configBuilder = DependencyResolver.Current.GetService<CmsConfigBuilder>();

            #region Admin Menus

            configBuilder.Global()
                .HasProjectName("KC Actors Theatre CMS")
                .HasMenuItems(new AdminMenuItem[]
                {
                    new AdminMenuItem 
                    { 
                        Title = "Blogs", 
                        Href = "#", 
                        RolesWithVisibility = new[] 
                        {
                            Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Manage Blog Categories", 
                                Href = "/CustomAdmin/PostCategories", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Manage Blog Tags", 
                                Href = "/CustomAdmin/Tags/Blog", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Blog: Daily Bread", 
                                Href = "/CustomAdmin/Blogs/DailyBread", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Blog: Evangelist", 
                                Href = "/CustomAdmin/Blogs/Evangelist", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                        } 
                    },
                    new AdminMenuItem 
                    { 
                        Title = "Events", 
                        Href = "#", 
                        RolesWithVisibility = new[] 
                        {
                            Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Manage Event Categories", 
                                Href = "/CustomAdmin/EventCategories", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Events", 
                                Href = "/CustomAdmin/Events", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                        } 
                    },
                    new AdminMenuItem 
                    { 
                        Title = "Tags", 
                        Href = "#", 
                        RolesWithVisibility = new[] 
                        {
                            Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "General Tags", 
                                Href = "/CustomAdmin/Tags/General", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Blog Tags", 
                                Href = "/CustomAdmin/Tags/Blog", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Mission Story Tags", 
                                Href = "/CustomAdmin/Tags/MissionStory", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                        } 
                    },
                    new AdminMenuItem 
                    { 
                        Title = "Mission Stories", 
                        Href = "/CustomAdmin/MissionStories", 
                        RolesWithVisibility = new[] 
                        {
                            Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                        } 
                    },
                    new AdminMenuItem
                    {
                        Title = "News",
                        Href = "#",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Announcements", 
                                Href = "/CustomAdmin/Announcements", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Official Notifications", 
                                Href = "/CustomAdmin/Announcements/Official", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                        }
                    },
                    new AdminMenuItem
                    {
                        Title = "Resources",
                        Href = "#",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Audio", 
                                Href = "/CustomAdmin/Resources/Audio", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Document", 
                                Href = "/CustomAdmin/Resources/Document", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "CD/DVD", 
                                Href = "/CustomAdmin/Resources/Media", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Presentation", 
                                Href = "/CustomAdmin/Resources/Presentation", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Product", 
                                Href = "/CustomAdmin/Resources/Product", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Publication", 
                                Href = "/CustomAdmin/Resources/Publication", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Slideshow", 
                                Href = "/CustomAdmin/Resources/Slideshow", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Video", 
                                Href = "/CustomAdmin/Resources/Video", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator 
                                } 
                            },
                        }
                    },
                    new AdminMenuItem
                    {
                        Title = "Staff Members",
                        Href = "#",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator,
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Staff", 
                                Href = "/CustomAdmin/StaffMembers/Staff", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Connect Widget Contacts", 
                                Href = "/CustomAdmin/StaffMembers/Connect", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                        }
                    },
                    new AdminMenuItem
                    {
                        Title = "Locations",
                        Href = "#",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator,
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Mission Fields", 
                                Href = "/CustomAdmin/MissionFields", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Mission Centers", 
                                Href = "/CustomAdmin/MissionCenters", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Congregations", 
                                Href = "/CustomAdmin/Congregations", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },                        
                        },
                    },
                    new AdminMenuItem
                    {
                        Title = "Organizations",
                        Href = "#",
                        RolesWithVisibility = new[]
                        {
                            Constants.CmsRole_SystemAdministrator,
                        },
                        Children = new AdminMenuItem[]
                        {
                            new AdminMenuItem 
                            { 
                                Title = "Affiliates", 
                                Href = "/CustomAdmin/Organizations/Affiliates", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },                        
                            new AdminMenuItem 
                            { 
                                Title = "Associations", 
                                Href = "/CustomAdmin/Organizations/Associations", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },                        
                            new AdminMenuItem 
                            { 
                                Title = "Ministries", 
                                Href = "/CustomAdmin/Organizations/Ministries", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                            new AdminMenuItem 
                            { 
                                Title = "Services", 
                                Href = "/CustomAdmin/Organizations/Services", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },
                            new AdminMenuItem 
                            { 
                                Title = "World Church Teams", 
                                Href = "/CustomAdmin/Organizations/WorldChurchTeams", 
                                RolesWithVisibility = new[] 
                                {
                                    Clickfarm.Cms.Constants.CmsRole_SystemAdministrator,
                                },
                            },                        
                        },
                    },
                })
            ;

            #endregion

            //page types
            configBuilder.PageType<WebPage>("Web Page");

            #region Machines

            configBuilder.DefaultMachine()
                .UsesCdnHost("clickfarmcdn.localhost")
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

            configBuilder.Content<TextContent>(CmsConfigConstants.ContentType_Text)
                .HasIconCssClass("ui-icon-comment")
                ;

            configBuilder.Content<HeroImageContent>(CmsConfigConstants.ContentType_HeroImage)
                .UsesView(CmsConfigConstants.EditView_HeroImage)
                .HasIconCssClass("ui-icon-image")
                .WithProperties(new FileContentProperties
                {
                    RootFolder = "/common/cms/images/inner/hero",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 770,
                    ExactHeight = 300
                })
            ;

            configBuilder.Content<LandingPageHeroImageContent>(CmsConfigConstants.ContentType_HeroImage)
                .UsesView(CmsConfigConstants.EditView_LandingPageHeroImage)
                .HasIconCssClass("ui-icon-image")
                .WithProperties(new FileContentProperties
                {
                    RootFolder = "/common/cms/images/landing/hero",
                    DefaultSubfolder = "",
                    MediaTypes = new[] {  "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 1200,
                    ExactHeight = 500
                })
            ;

            configBuilder.Content<RotatorImageContent>(CmsConfigConstants.ContentType_RotatorImage)
                .UsesView(CmsConfigConstants.EditView_RotatorImage)
                .HasIconCssClass("ui-icon-image")
                .WithProperties(new FileContentProperties
                {
                    RootFolder = "/common/cms/images/rotator",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 1200,
                    ExactHeight = 500
                })
            ;

            configBuilder.Content<ContentWidgetContent>(CmsConfigConstants.ContentType_ContentWidget)
                .UsesView(CmsConfigConstants.EditView_ContentWidget)
                .HasIconCssClass("ui-icon-image")
                .WithProperties(new FileContentProperties
                {
                    RootFolder = "/common/cms/images/content-widget",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 350,
                    ExactHeight = 200
                })
            ;

            configBuilder.Content<ConnectWidgetContent>(CmsConfigConstants.ContentType_ConnectWidget)
                .UsesView(CmsConfigConstants.EditView_ConnectWidget)
                .HasIconCssClass("ui-icon-image")
            ;

            configBuilder.Content<ResourceWidgetContent>(CmsConfigConstants.ContentType_ResourcesWidget)
                .UsesView(CmsConfigConstants.EditView_ResourceWidget)
                .HasIconCssClass("ui-icon-image")
            ;

            configBuilder.Content<AnnouncementWidgetContent>(CmsConfigConstants.ContentType_AnnouncementsWidget)
                .UsesView(CmsConfigConstants.EditView_AnnouncementWidget)
                .HasIconCssClass("ui-icon-image")
            ;

            configBuilder.Content<MissionStoryWidgetContent>(CmsConfigConstants.ContentType_MissionStoriesWidget)
                .UsesView(CmsConfigConstants.EditView_MissionStoryWidget)
                .HasIconCssClass("ui-icon-image")
            ;

            // home page content types
            configBuilder.Content<AlertContent>(CmsConfigConstants.ContentType_AlertContent)
                .UsesView(CmsConfigConstants.EditView_AlertContent)
                .HasIconCssClass("ui-icon-image")
            ;

            configBuilder.Content<HomePageMissionStoryWidgetContent>(CmsConfigConstants.ContentType_MissionStoriesWidget)
                .UsesView(CmsConfigConstants.EditView_HomePageMissionStoryWidget)
                .HasIconCssClass("ui-icon-image")
            ;
            configBuilder.Content<HomePageContentWidgetContent>(CmsConfigConstants.ContentType_ContentWidget)
                .UsesView(CmsConfigConstants.EditView_HomePageContentWidget)
                .HasIconCssClass("ui-icon-image")
                .WithProperties(new FileContentProperties
                {
                    RootFolder = "/common/cms/images/content-widget",
                    DefaultSubfolder = "",
                    MediaTypes = new[] { "image/" }
                })
                .WithProperties(new ImageContentProperties
                {
                    ExactWidth = 350,
                    ExactHeight = 200
                })
            ;

            #endregion

            #region Content groups

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_BannerText)
                .AsFixed()
                .HasContentType<TextContent>(CmsConfigConstants.ContentType_Text)
                ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_BodyCopy)
                .AsFixed()
                .HasContentType<HtmlContent>(CmsConfigConstants.ContentType_BodyCopy)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .AsFixed()
                .HasContentType<ResourceWidgetContent>(CmsConfigConstants.ContentType_ResourcesWidget)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .AsFixed()
                .HasContentType<AnnouncementWidgetContent>(CmsConfigConstants.ContentType_AnnouncementsWidget)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_ConnectWidget)
                .AsFixed()
                .HasContentType<ConnectWidgetContent>(CmsConfigConstants.ContentType_ConnectWidget)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_MissionStoriesWidget)
                .AsFixed()
                .HasContentType<MissionStoryWidgetContent>(CmsConfigConstants.ContentType_MissionStoriesWidget)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_ContentWidgets)
                .HasContentType<ContentWidgetContent>(CmsConfigConstants.ContentType_ContentWidget)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_LandingPageHeroImage)
                .AsFixed()
                .HasContentType<LandingPageHeroImageContent>(CmsConfigConstants.ContentType_HeroImage)
            ;

            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_HeroImage)
                .AsFixed()
                .HasContentType<HeroImageContent>(CmsConfigConstants.ContentType_HeroImage)
            ;

            // home page content groups
            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_Alerts)
                .HasContentType<AlertContent>(CmsConfigConstants.ContentType_AlertContent)
            ;
            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_RotatorImages)
                .HasContentType<RotatorImageContent>(CmsConfigConstants.ContentType_RotatorImage)
            ;
            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_HomePageMissionStoriesWidget)
                .AsFixed()
                .HasContentType<HomePageMissionStoryWidgetContent>(CmsConfigConstants.ContentType_MissionStoriesWidget)
            ;
            configBuilder.ContentGroup(CmsConfigConstants.ContentGroup_HomePageContentWidget)
                .AsFixed()
                .HasContentType<HomePageContentWidgetContent>(CmsConfigConstants.ContentType_ContentWidget)
            ;

            #endregion

            #region English Website

            AppConfiguration app = configBuilder.App("English Website")
                .HasPageType(CmsConfigConstants.PageType_WebPage)
                .UsesEditView("_KCActorsTheatreAppEdit")
            ;
            app.OnDefaultMachine()
                .HasHost<CustomCachedRequestContentHandler>("localhost", "Live")
            ;
            app.OnMachine(Constants.MachineName_DevWeb)
                .HasHost<CustomCachedRequestContentHandler>("cofchrist.clickfarminteractive.com", "Live")
            ;
            app.OnMachine(Constants.MachineName_ProdWeb)
                .HasHost<CustomCachedRequestContentHandler>("www.cofchrist.org", "Live", isPrimary: true)
                .HasHost<CustomCachedRequestContentHandler>("67.59.163.21", "IP")
            ;

            app.HasController<Controllers.HomeController>("Home Page", "Index")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_Alerts)
                .HasContentGroup(CmsConfigConstants.ContentGroup_BannerText)
                .HasContentGroup(CmsConfigConstants.ContentGroup_RotatorImages)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageMissionStoriesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageContentWidget)
                .SingleUse()
            ;

            app.HasController<Controllers.HomeController>("Landing Page", "Landing")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_LandingPageHeroImage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_BodyCopy)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ContentWidgets)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ConnectWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_MissionStoriesWidget)
            ;

            app.HasController<Controllers.HomeController>("Inner Page", "Inner")
                .IsDefault()
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HeroImage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_BodyCopy)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_MissionStoriesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ContentWidgets)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ConnectWidget)
            ;

            app.HasController<Controllers.NewsController>("Announcements", "Announcements")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("Official Announcements", "OfficialAnnouncements")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("Mission Stories", "MissionStories")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .SingleUse()
                ;

            app.HasController<Controllers.NewsController>("Mission Story Detail Page", "MissionStory")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .SingleUse()
                ;

            app.HasController<Controllers.BlogController>("Blog - Daily Bread", "DailyBread")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ContentWidgets)
                .SingleUse()
                ;

            app.HasController<Controllers.BlogController>("Blog - Evangelist", "Evangelist")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ContentWidgets)
                .SingleUse()
                ;

            app.HasController<Controllers.HomeController>("Site Map", "SiteMap")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .SingleUse()
            ;

            app.HasController<Controllers.SearchController>("Search", "Index")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_BodyCopy)
                .SingleUse()
            ;

            #endregion

            #region Spanish Website

            AppConfiguration spanishApp = configBuilder.App("Spanish Website")
                .HasPageType(CmsConfigConstants.PageType_WebPage)
                .UsesEditView("_KCActorsTheatreAppEdit")
            ;
            spanishApp.OnDefaultMachine()
                .HasHost<PreviewRequestContentHandler>("es.localhost", "Live")
            ;
            spanishApp.OnMachine(Constants.MachineName_DevWeb)
                .HasHost<PreviewRequestContentHandler>("espanol.cofchrist.clickfarminteractive.com", "Live")
            ;
            spanishApp.OnMachine(Constants.MachineName_ProdWeb)
                .HasHost<CachedRequestContentHandler>("espanol.cofchrist.org", "Live", isPrimary: true)
                .HasHost<CachedRequestContentHandler>("67.59.163.21", "IP")
            ;

            spanishApp.HasController<Controllers.HomeController>("Home Page", "Index")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_RotatorImages)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageMissionStoriesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageContentWidget)
                .SingleUse()
            ;

            #endregion

            #region French Website

            AppConfiguration frenchApp = configBuilder.App("French Website")
                .HasPageType(CmsConfigConstants.PageType_WebPage)
                .UsesEditView("_KCActorsTheatreAppEdit")
            ;
            frenchApp.OnDefaultMachine()
                .HasHost<PreviewRequestContentHandler>("fr.localhost", "Live")
            ;
            frenchApp.OnMachine(Constants.MachineName_DevWeb)
                .HasHost<PreviewRequestContentHandler>("francais.cofchrist.clickfarminteractive.com", "Live")
            ;
            frenchApp.OnMachine(Constants.MachineName_ProdWeb)
                .HasHost<CachedRequestContentHandler>("francais.cofchrist.org", "Live", isPrimary: true)
                .HasHost<CachedRequestContentHandler>("67.59.163.21", "IP")
            ;

            frenchApp.HasController<Controllers.HomeController>("Home Page", "Index")
                .ForPageType(CmsConfigConstants.PageType_WebPage)
                .HasContentGroup(CmsConfigConstants.ContentGroup_RotatorImages)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageMissionStoriesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_ResourcesWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_AnnouncementsWidget)
                .HasContentGroup(CmsConfigConstants.ContentGroup_HomePageContentWidget)
                .SingleUse()
            ;

            #endregion
        }
    }
}
