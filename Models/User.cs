using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp23_CV.Models
{

    //User ansvarar för autentisering och grundläggande användaruppgifter som inloggning, registrering och privat/offentlig profilstatus.
    public class User : IdentityUser<int>
    { 

            [Required]
            [Display(Name = "Privat profil")]
            public bool IsPrivate { get; set; } // Privat/offentlig profil

            //[ForeignKey(nameof(CvId))]
            //public int CvId { get; set; }// Är användbar för queries där du endast behöver ID:t.
            //public virtual CV CV { get; set; } // Navigationsegenskap för relationen till CV
                                       //OSÄKER PÅ OM BÅDA BEHÖVS, ELLER ENDAST EN AV DEM??
        public virtual List<UserProject> User_Projects { get; set; } = new(); // Koppling till projekt
    }
}
    
