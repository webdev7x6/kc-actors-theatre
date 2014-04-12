using KCActorsTheatre.Staff;
using System;
using System.Collections.Generic;

namespace KCActorsTheatre.Locations
{
    public class MissionField
    {
        public int MissionFieldID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Name { get; set; }

        private HashSet<MissionCenter> _missionCenters = new HashSet<MissionCenter>();
        public ICollection<MissionCenter> MissionCenters { get { return _missionCenters; } }

        private HashSet<RoleDefinition> _roleDefinitions = new HashSet<RoleDefinition>();
        public ICollection<RoleDefinition> RoleDefinitions { get { return _roleDefinitions; } }
    }
}
