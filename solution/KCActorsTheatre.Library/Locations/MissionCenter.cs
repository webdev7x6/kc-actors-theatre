using KCActorsTheatre.Staff;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KCActorsTheatre.Locations
{
    public class MissionCenter
    {
        public int MissionCenterID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public PhoneType? PhoneType1 { get; set; }
        public string Phone1 { get; set; }

        public PhoneType? PhoneType2 { get; set; }
        public string Phone2 { get; set; }

        public PhoneType? PhoneType3 { get; set; }
        public string Phone3 { get; set; }

        public int MissionFieldID { get; set; }
        public MissionField MissionField { get; set; }

        private HashSet<RoleDefinition> _roleDefinitions = new HashSet<RoleDefinition>();
        public ICollection<RoleDefinition> RoleDefinitions { get { return _roleDefinitions; } }
    }

    public enum PhoneType
    {
        [Description("Phone")]
        Phone,
        [Description("Fax")]
        Fax,
    }
}