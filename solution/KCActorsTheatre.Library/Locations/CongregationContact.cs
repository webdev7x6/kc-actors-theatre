using System;

namespace KCActorsTheatre.Locations
{
    public class CongregationContact
    {
        public int CongregationContactID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? DisplayOrder { get; set; }

        public int CongregationID { get; set; }
        public Congregation Congregation { get; set; }
    }
}