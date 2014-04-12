using Clickfarm.Cms.Core;
using KCActorsTheatre.Staff;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KCActorsTheatre.Cms.ContentTypes
{
    public class ConnectWidgetContent : Content
    {
        private HashSet<ConnectWidgetStaffMember> _connectWidgetStaffMembers = new HashSet<ConnectWidgetStaffMember>();
        public ICollection<ConnectWidgetStaffMember> ConnectWidgetStaffMembers { get { return _connectWidgetStaffMembers; } }

        public override string ToHtmlString()
        {
            return string.Empty;
        }
    }

    public class ConnectWidgetStaffMember
    {
        [Key, Column(Order = 0)]
        public int ContentID { get; set; }

        [Key, Column(Order = 1)]
        public int StaffMemberID { get; set; }
        public StaffMember StaffMember { get; set; }

        public int DisplayOrder { get; set; }
    }
}