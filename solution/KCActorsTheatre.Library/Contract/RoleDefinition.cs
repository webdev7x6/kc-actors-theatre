using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Contract
{
    public class RoleDefinition
    {
        public int RoleDefinitionID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Title { get; set; }

        [ForeignKey("Person")]
        public int PersonID { get; set; }
        public Person Person { get; set; }

        [ForeignKey("Show")]
        public int ShowID { get; set; }
        public ShowInfo Show { get; set; }

    }
}
