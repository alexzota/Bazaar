using GucciBazaar.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GucciBazaar.Models
{
    public class ProductAdditionRequest : BaseModel
    {
        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(maximumLength: 250, ErrorMessage = "Titlul nu poate avea mai mult de 250 de caractere")]
        public string Title { get; set; }

        [StringLength(1000)]
        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public long CategoryId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(128)]
        public string ApproverId { get; set; }

        [StringLength(2000)]
        public string ImagePath { get; set; }
        public double Price { get; set; }

        public StateTypes CurrentState { get; set; }


        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationUser Approver { get; set; }
        public virtual IEnumerable<SelectListItem> Categories { get; set; }

        public ProductAdditionRequest()
        {
            CurrentState = StateTypes.Pending;
        }
    }
}