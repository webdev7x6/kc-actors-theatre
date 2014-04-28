using System;

namespace KCActorsTheatre.Contract
{
    public class ShowImage
    {
        public int ShowImageID { get; set; }
        public DateTime DateCreated { get; set; }
        public string ImageURL { get; set; }
        public int DisplayOrder { get; set; }
        public int ShowID { get; set; }
        public ShowInfo Show { get; set; }
    }
}