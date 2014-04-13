using Clickfarm.Cms.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Library.AppTypes
{
    public class KCActorsTheatreApp : App
    {
        public string ContactUsEmail { get; set; }
        public string VolunteerEmail { get; set; }
        public string FacebookURL { get; set; }
        public string PinterestURL { get; set; }
        public string YouTubeURL { get; set; }
    }
}