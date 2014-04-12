using Clickfarm.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Cms.AppTypes
{
    public class KCActorsTheatreApp : App
    {
        public string BlogCommmentEmailNotifications { get; set; }
        public string FacebookURL { get; set; }
        public string PinterestURL { get; set; }
        public string YouTubeURL { get; set; }
        public string NewsletterSignUpForm { get; set; }
    }
}