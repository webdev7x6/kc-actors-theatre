using Clickfarm.Cms.Core;
using Clickfarm.Cms.Mvc;
using KCActorsTheatre.Data;
using KCActorsTheatre.Library.AppTypes;
using System.Collections.Generic;


namespace KCActorsTheatre.Web.ViewModels
{
    public class ContactViewModel : KCActorsTheatreViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MailingAddress { get; set; }
        public string Comments { get; set; }
    }
}