using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp23_CV.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Jobbtitel")]
        public string? JobTitle { get; set; }

        public int CvId { get; set; }// Är användbar för queries där du endast behöver ID:t.

        [ForeignKey(nameof(CvId))]
        public virtual CV? CV { get; set; } // Navigationsegenskap för relationen till CV
    }
}
