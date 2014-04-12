using KCActorsTheatre.Locations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Staff
{
    public class RoleDefinition
    {
        public int RoleDefinitionID { get; set; }
        public DateTime DateCreated { get; set; }
        public Role Role { get; set; }
        public string Title { get; set; }
        public int? DisplayOrder { get; set; }

        public int StaffMemberID { get; set; }
        public virtual StaffMember StaffMember { get; set; }

        public int? MissionCenterID { get; set; }
        public virtual MissionCenter MissionCenter { get; set; }

        public int? MissionFieldID { get; set; }
        public virtual MissionField MissionField { get; set; }
    }
}