using System.ComponentModel.DataAnnotations;

namespace GucciBazaar.Models
{
    public class ShoppingCartProduct : BaseModel
    {
        public long ShoppingCartId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual Product Product { get; set; }
    }
}