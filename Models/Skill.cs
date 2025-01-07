using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Skill
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Kompetens")]
        public string Name { get; set; } // T.ex "java", "c#", "projektledning"

        //[Display(Name = "Nivå")]
        //public string Level { get; set; } // Exempel: Grundläggande nivå, "Medelkompetens", "Hög kompetens" 

        //[Display(Name = "Beskrivning")]
        //public string Description { get; set; } // Kort beskrivning av kompetensen

        [Required]
       
        public int CvId { get; set; }// Är användbar för queries där du endast behöver ID:t.
        public virtual CV CV { get; set; } // Navigationsegenskap för relationen till CV
                                   //OSÄKER PÅ OM BÅDA BEHÖVS, ELLER ENDAST EN AV DEM??


    }
}
