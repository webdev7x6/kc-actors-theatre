using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using Clickfarm.Cms.Core;
using Clickfarm.Cms.Data.EntityFramework;
using KCActorsTheatre.Cms.AppTypes;
using Clickfarm.Cms;
using KCActorsTheatre.Data;
using KCActorsTheatre;
using KCActorsTheatre.Locations;
using KCActorsTheatre.Resources;
using KCActorsTheatre.Staff;
using KCActorsTheatre.Tags;
using System.Data.Entity.Core.Objects;

namespace KCActorsTheatre.Data
{
    public class KCActorsTheatreDatabaseInitializer : CmsDatabaseInitializer
    {
        protected override void Seed(EntityFrameworkCmsDbContext context)
        {
            base.Seed(context);

            ObjectContext ocx = ((IObjectContextAdapter)context).ObjectContext;

            KCActorsTheatreApp appOne = App<KCActorsTheatreApp>("cofchrist.org", "Central Standard Time", AppType.Web);

            //Hosts for cofchrist.org
            PrimaryHost companySitePrimaryHost = PrimaryHost("www.cofchrist.org", appOne);
            SecondaryHost("cofchrist.org", appOne, companySitePrimaryHost);
            SecondaryHost("preview-www.cofchrist.org", appOne, companySitePrimaryHost, true);

            PrimaryHost localhost = PrimaryHost("localhost", appOne);
            SecondaryHost("preview-localhost", appOne, localhost, true);

            Url homePageUrl;
            WebPage homePage = WebPage(appOne, out homePageUrl, "Home Page", "Community of Christ", "/", "Home Page");
            homePage.MetaDescription = "";
            homePage.MetaKeywords = "";

            //Primary Navigation
            MenuCategory category = MenuCategory("Navigation", appOne);
            Menu primaryNav = Menu(appOne, "Primary Navigation", "The main navigational element of the site.", new MenuCategory[] { category }, "/", 0, true);

            //Utility Navigation
            Menu utilityNav = Menu(appOne, "Utility Navigation", "The utiltity navigation appearing in the header of the site.", new MenuCategory[] { category }, "/", 1, false);
            MenuItem homeNode = UrlMenuItem(homePageUrl, utilityNav, "Home", 0);

            Url contactUsUrl;
            WebPage(appOne, out contactUsUrl, "Contact Us", "Contact Community of Christ", "/contact-us", "Inner Page", "/contact-us", "Alias");
            MenuItem cotactUsNode = UrlMenuItem(contactUsUrl, utilityNav, "Contact Us", 1);

            //Footer Navigation
            Menu footerNav = Menu(appOne, "Footer Navigation", "The navigation appearing in the footer of the site.", new MenuCategory[] { category }, "/", 2, false);

            Url siteMapUrl;
            WebPage(appOne, out siteMapUrl, "Site Map", "Site Map", "/site-map", "Site Map", "/site-map", "Alias");
            MenuItem siteMapNode = UrlMenuItem(siteMapUrl, footerNav, "Site Map", 0);

            Url legalUrl;
            WebPage(appOne, out legalUrl, "Legal", "Legal", "/legal", "Inner Page", "/legal", "Alias");
            MenuItem legalNode = UrlMenuItem(legalUrl, footerNav, "Legal", 1);

            Url termsUrl;
            WebPage(appOne, out termsUrl, "Terms of Use", "Terms of Use", "/terms-of-use", "Inner Page", "/terms-of-use", "Alias");
            MenuItem termsNode = UrlMenuItem(termsUrl, footerNav, "Terms of Use", 2);

            Url privacyUrl;
            WebPage(appOne, out privacyUrl, "Privacy Policy", "Privacy Policy", "/privacy-policy", "Inner Page", "/privacy-policy", "Alias");
            MenuItem privacyNode = UrlMenuItem(privacyUrl, footerNav, "Privacy Policy", 3);

            // User, Site Admin, Super Admin, HR, News Admin

            // Super Admin
            CmsRole roleSysAdmin = CmsRole(Clickfarm.Cms.Constants.CmsRole_SystemAdministrator);
            CmsRole roleContMgr = CmsRole(Clickfarm.Cms.Constants.CmsRole_ContentManager);

            // User
            CmsRole roleUserMgr = CmsRole(Clickfarm.Cms.Constants.CmsRole_UserManager);
            CmsRole membershipMgr = CmsRole(Clickfarm.Cms.Constants.CmsRole_MembershipManager);

            CmsUser("matt.lawson@clickfarminteractive.com", "Matt", "Lawson", "blah1234", "Central Standard Time", new CmsRole[] { roleSysAdmin });

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                var errors = context.GetValidationErrors();
                throw;
            }


            KCActorsTheatreDbContext dbContext = (KCActorsTheatreDbContext)context;

            // Mission Centers
            var MissionCenters = new List<MissionCenter>();

            var missionCenter1 = new MissionCenter()
            {
                Address1 = "123 Sesame St",
                Address2 = "Suite 101",
                City = "Your town",
                Country = "USA",
                DateCreated = DateTime.UtcNow,
                Email = "me@you.com",
                Name = "Test Mission Center",
                Phone1 = "816-555-1212",
                Phone2 = "816-111-2222",
                Phone3 = "913-648-8888",
                PhoneType1 = PhoneType.Phone,
                PhoneType2 = PhoneType.Fax,
                PhoneType3 = PhoneType.Phone,
                PostalCode = "12345",
                State = "Missouri",
                Website = "http://www.google.com"
            };
            MissionCenters.Add(missionCenter1);

            var missionCenter2 = new MissionCenter()
            {
                Address1 = "456 Sesame St",
                Address2 = "Suite 777",
                City = "Kansas City",
                Country = "USA",
                DateCreated = DateTime.UtcNow,
                Email = "test@test.com",
                Name = "Another Test Mission Center",
                Phone1 = "816-888-8888",
                Phone2 = "123-456-7890",
                Phone3 = "555-555-5555",
                PhoneType1 = PhoneType.Phone,
                PhoneType2 = PhoneType.Fax,
                PhoneType3 = PhoneType.Phone,
                PostalCode = "64108",
                State = "Kansas",
                Website = "http://www.google.com"
            };
            MissionCenters.Add(missionCenter2);

            // Resources and Resource Tags
            var Resources = new List<Resource>();

            var resource1 = new Resource()
            {
                DateCreated = DateTime.UtcNow,
                Description = "Resource Description",
                Language = Language.English,
                ResourceType = ResourceType.Audio,
                Tags =
                {
                    new Tag() 
                    {
                        DateCreated = DateTime.UtcNow,
                        Name = "Test Tag",
                    },
                    new Tag() 
                    {
                        DateCreated = DateTime.UtcNow,
                        Name = "Another Tag",
                    },
                },
                Title = "Test Resource Title",
                URL = "http://www.google.com"
            };
            Resources.Add(resource1);

            var resource2 = new Resource()
            {
                DateCreated = DateTime.UtcNow,
                Description = "Another Resource Description",
                Language = Language.English,
                ResourceType = ResourceType.Publication,
                Tags =
                {
                    new Tag() 
                    {
                        DateCreated = DateTime.UtcNow,
                        Name = "3rd test tag",
                    },
                    new Tag() 
                    {
                        DateCreated = DateTime.UtcNow,
                        Name = "4th test tag",
                    },
                },
                Title = "Another Test Resource Title",
                URL = "http://www.google.com"
            };
            Resources.Add(resource2);

            // staff members and role definitions
            var StaffMembers = new List<StaffMember>();

            var staffMember1 = new StaffMember()
            {
                Biography = "biography",
                DateCreated = DateTime.UtcNow,
                Email = "me@you.com",
                //FieldRoleDefinitions =
                //{
                //    new FieldRoleDefinition() 
                //    {
                //        DateCreated = DateTime.UtcNow,
                //        Description = "Field role description",
                //        Field = new Field() 
                //        {
                //            Name = "Test Field"
                //        },
                //        Role = Role.Field
                //    }
                //},
                //ImageURL = "",
                //MissionCenterRoleDefinitions =
                //{
                //    new MissionCenterRoleDefinition() 
                //    {
                //        DateCreated = DateTime.UtcNow,
                //        Description = "Field role description",
                //        MissionCenter = missionCenter1,
                //        Role = Role.MissionCenter
                //    },
                //    new MissionCenterRoleDefinition() 
                //    {
                //        DateCreated = DateTime.UtcNow,
                //        Description = "Field role description 2",
                //        MissionCenter = missionCenter2,
                //        Role = Role.MissionCenter
                //    }
                //},
                //Name = "John Smith",
                //OtherRoleDefinitions =
                //{
                //    new OtherRoleDefinition() 
                //    {
                //        DateCreated = DateTime.UtcNow,
                //        Description = "Field role description",
                //        OtherRoleType = OtherRoleType.Affiliate,
                //        Role = Role.Other
                //    }
                //},
                Phone = "816-816-8168"
            };
            StaffMembers.Add(staffMember1);

            try
            {
                // add mission centers
                foreach (var item in MissionCenters)
                    dbContext.MissionCenters.Add(item);

                // add resources and tags
                foreach (var item in Resources)
                    dbContext.Resources.Add(item);

                // add staff members and role definitions
                foreach (var item in StaffMembers)
                    dbContext.StaffMembers.Add(item);

                dbContext.SaveChanges();
                context.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                var errors = dbContext.GetValidationErrors();
                throw;
            }
        }
    }
}
