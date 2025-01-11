using Grupp23_CV.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Grupp23_CV.Models
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; } // Unik identifierare för projektet

        [Required(ErrorMessage = "Vänligen ange namn för projektnamn")]
        [Display(Name = "Projektnamn")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vänligen ange en beskrivning av projektet")]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        //public virtual User User { get; set; }
        public virtual List<UserProject> UserProjects { get; set; } = new(); // Koppling till användare
    }


}

