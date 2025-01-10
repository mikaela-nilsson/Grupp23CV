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

        [Required]
        [Display(Name = "Projektnamn")]
        public string Title { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        //public virtual User User { get; set; }
        public virtual List<UserProject> User_Projects { get; set; } = new(); // Koppling till användare
    }


}

