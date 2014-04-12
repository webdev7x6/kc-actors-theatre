using KCActorsTheatre.Resources;
using KCActorsTheatre.Tags;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Organizations
{
    public class Organization
    {
        public int OrganizationID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public bool IsPublished { get; set; }

        public string Name { get; set; }

        public string LinkTarget { get; set; }
        public string LinkURL { get; set; }

        public OrganizationType OrganizationType { get; set; }
    }
}