using KCActorsTheatre.Cms.ContentTypes;
using System;
using System.Collections.Generic;

namespace KCActorsTheatre.Staff
{
    public class StaffMember
    {
        public int StaffMemberID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SortName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string ImageURL { get; set; }
        public string Biography { get; set; }

        public StaffType StaffType { get; set; }

        private HashSet<RoleDefinition> _roleDefinitions = new HashSet<RoleDefinition>();
        public ICollection<RoleDefinition> RoleDefinitions { get { return _roleDefinitions; } }
    }

    public class ConnectWidgetContact : StaffMember
    {
        public ConnectWidgetContact() { StaffType = StaffType.ConnectWidgetContact; }
    }

    public class RegularStaffMember : StaffMember
    {
        public RegularStaffMember() { StaffType = StaffType.StaffMember; }
    }
}