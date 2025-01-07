using System.ComponentModel.DataAnnotations;

namespace Aiges.MVC.Models
{
    public class AddCommentViewModel
    {     
        public int? ProjectId { get; set; }

        public int? EventId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Comment { get; set; }
    }
}
