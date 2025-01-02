using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
  
        public class User_Project
        {
            public int UserId { get; set; } // Främmande nyckel till User
            public User User { get; set; } // Navigationsegenskap till User

            public int ProjectId { get; set; } // Främmande nyckel till Project
            public Project Project { get; set; } // Navigationsegenskap till Project

            [Display(Name = "Roll i projektet")]
            public string Role { get; set; } // Exempel: "Projektledare", "Deltagare"

            [Display(Name = "Ansvar")]
            public string Responsibility { get; set; } // Valfritt: Beskrivning av ansvaret i projektet
        }

    }

