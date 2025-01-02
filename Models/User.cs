using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{

    //User ansvarar för autentisering och grundläggande användaruppgifter som inloggning, registrering och privat/offentlig profilstatus.
    public class User
    {
      
            [Key]
            public int UserId { get; set; } // Unik identifierare för användaren

            [Required]
            [Display(Name = "Användarnamn")]
            public string UserName { get; set; } // Användarnamn för inloggning

            [Required]
            [Display(Name = "E-postadress")]
            [EmailAddress]
            public string Email { get; set; } // E-postadress för kontakt och inloggning

            [Required]
            [Display(Name = "Lösenord")]
            public string PasswordHash { get; set; } // Hashat lösenord

            [Required]
            [Display(Name = "Privat profil")]
            public bool IsPrivate { get; set; } // Privat/offentlig profil

            public int CvId { get; set; }// Är användbar för queries där du endast behöver ID:t.
            public CV CV { get; set; } // Navigationsegenskap för relationen till CV
                                       //OSÄKER PÅ OM BÅDA BEHÖVS, ELLER ENDAST EN AV DEM??
        public List<UserProject> User_Projects { get; set; } = new(); // Koppling till projekt
    }
}
    
