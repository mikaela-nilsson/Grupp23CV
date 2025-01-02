using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Jobbtitel")]
        public string JobTitle { get; set; }

        [Required]
        [Display(Name = "Företag")]
        public string Company { get; set; }

        [Required]
        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime EndDate { get; set; } //Vi tolkar det som att det inte finnas något Nullable värde, dvs tidigare erfarenhet, ska redan vara avslutat. Eller så kan vi ha nullable.
        
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Required]
        public int CvId { get; set; }


    }
}
