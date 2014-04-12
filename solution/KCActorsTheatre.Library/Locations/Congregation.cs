using System;
using System.Collections.Generic;

namespace KCActorsTheatre.Locations
{
    public class Congregation
    {
        public int CongregationID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string Name { get; set; }
        public string ImageURL { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public int? MissionCenterID { get; set; }
        public MissionCenter MissionCenter { get; set; }

        private HashSet<CongregationContact> _congregationContacts = new HashSet<CongregationContact>();
        public ICollection<CongregationContact> CongregationContacts { get { return _congregationContacts; } }
    }
}