using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp23_CV.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Utbildning")]
        public string? Name { get; set; }

        public int CvId { get; set; }

        [ForeignKey(nameof(CvId))]
        public virtual CV? CV { get; set; }

    }
}
