using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCActorsTheatre.Contract
{
    public class Article
    {
        public int ArticleID { get; set; }
        public DateTime DateCreated { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        public DateTime? ArticleDate { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public bool IsPublished { get; set; }

        public string ShortDate
        {
            get { return ArticleDate.HasValue ? ArticleDate.Value.ToShortDateString() : ""; }
        }

        public string ExtendedDate
        {
            get { return ArticleDate.HasValue ? ArticleDate.Value.ToString("MMMM dd, yyyy") : ""; }
        }

        public string AuthorOrDefault
        {
            get { return string.IsNullOrWhiteSpace(Author) ? "Kansas City Actors Theatre" : Author; }
        }
    }
}