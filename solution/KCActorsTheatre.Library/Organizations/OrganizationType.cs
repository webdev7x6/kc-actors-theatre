using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Organizations
{
    public enum OrganizationType
    {
        [Description("Affiliate")]
        Affiliate,
        [Description("Association")]
        Association,
        [Description("Ministry")]
        Ministry,
        [Description("Service")]
        Service,
        [Description("World Church Team")]
        WorldChurchTeam,
    }
}