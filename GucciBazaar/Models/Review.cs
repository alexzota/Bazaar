using System.ComponentModel.DataAnnotations;

namespace GucciBazaar.Models
{
    public class Review : BaseModel
    {
        [StringLength(128)]
        public string UserId { get; set; }

        public long ProductId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }
        public double Rating { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}