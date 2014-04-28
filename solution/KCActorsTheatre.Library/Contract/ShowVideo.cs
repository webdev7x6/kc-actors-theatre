using System;

namespace KCActorsTheatre.Contract
{
    public class ShowVideo
    {
        public int ShowVideoID { get; set; }
        public DateTime DateCreated { get; set; }
        public string VimeoID { get; set; }
        public int DisplayOrder { get; set; }
        public int ShowID { get; set; }
        public ShowInfo Show { get; set; }
    }
}