using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GucciBazaar.Models
{
    public class ShoppingCart : BaseModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        public virtual ICollection<ShoppingCartProduct> Products { get; set; }
        public virtual ApplicationUser User { get; set; }

        public ShoppingCart()
        {
            Products = new List<ShoppingCartProduct>();
        }
    }
}