using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GucciBazaar.Models
{
    public class Product : BaseModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(maximumLength: 250, ErrorMessage = "Titlul nu poate avea mai mult de 250 de caractere")]
        public string Title { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Descrierea produsului este obligatorie")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public long CategoryId { get; set; }

        public double Price { get; set; }
        public double Rating { get; set; }

        [StringLength(2000)]
        public string ImagePath { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual IEnumerable<SelectListItem> Categories { get; set; }        public Product()
        {
            Reviews = new List<Review>();
        }    }
}