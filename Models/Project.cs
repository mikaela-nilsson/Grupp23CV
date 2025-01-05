using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; } // Unik identifierare för projektet

        [Required]
        [Display(Name = "Projektnamn")]
        public string Title { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Startdatum")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Slutdatum")]
        public DateTime? EndDate { get; set; } // Nullable, om projektet inte är avslutat

        public virtual List<UserProject> User_Projects { get; set; } = new (); // Koppling till användare
}


}

