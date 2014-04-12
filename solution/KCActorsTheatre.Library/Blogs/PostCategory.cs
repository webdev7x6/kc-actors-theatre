using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Blogs
{
    public class PostCategory
    {
        [Required]
        public int PostCategoryID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        [DataType(DataType.Text)]
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        public ICollection<Post> Posts { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}