using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCActorsTheatre.Calendar
{
    public class EventCategory
    {
        [Required]
        public int EventCategoryID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }

        [DataType(DataType.Text)]
        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        public ICollection<CalendarEvent> CalendarEvents { get; private set; }

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