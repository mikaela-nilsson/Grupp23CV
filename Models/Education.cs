using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Utbildning")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Institution")]
        public string Institution { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }
        
        [Display(Name = "Slutdatum")]
        public DateTime? EndDate { get; set; } // Nullable om utbildningen t.ex inte är avslutad, då sätter man ett "?" framför.

        [Required]
       
        public int CvId { get; set; }// Är användbar för queries där du endast behöver ID:t.
        public CV CV { get; set; } // Navigationsegenskap för relationen till CV
                                   //OSÄKER PÅ OM BÅDA BEHÖVS, ELLER ENDAST EN AV DEM??



    }
}
