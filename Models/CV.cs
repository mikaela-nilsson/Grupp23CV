using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    //CV ansvarar för att lagra detaljerad profilinformation som kontaktuppgifter, kompetenser, utbildningar, erfarenheter och projekt.
    public class CV
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fullständiga namn")]
        public string FullName { get; set; }

        [Display(Name = "Adress")]
        public string Adress { get; set; }

        [Display(Name = "Telefonnummer")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Profilbild")]
        public string ProfileImagePath { get; set; }

        // Relationer till andra entiteter. Ett cv kan innehålla flera skills, educations, exeperiences. Dvs 1:N samband. Vet ej om man kan ha med detta här under??
        public virtual List<Skill> Skills { get; set; } = new(); // Kompetenser
        public virtual List<Education> Educations { get; set; } = new(); // Utbildningar
        public virtual List<Experience> Experiences { get; set; } = new(); // Erfarenheter


    }
}
