using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre
{
    public class NewsletterSignUp
    {
        public int NewsletterSignUpID { get; set; }
        public DateTime DateCreated { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "E-mail address is not in correct format")]
        [Required(ErrorMessage = "E-mail address is required.")]
        public string EmailAddress { get; set; }
    }
}