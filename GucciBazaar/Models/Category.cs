using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GucciBazaar.Models
{
    public class Category : BaseModel
    {
        [StringLength(250)]
        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}