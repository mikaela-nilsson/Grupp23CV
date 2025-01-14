using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Nuvarande lösenord får inte vara tomt")]
        [DataType(DataType.Password)]
        [Display(Name = "Nuvarande Lösenord")]
        public string NuvarandeLosenord { get; set; }

        [Required(ErrorMessage = "Nytt lösenord får inte vara tomt")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Nya lösenordet måste innehålla minst {1} tecken", MinimumLength = 6)]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+", ErrorMessage = "Lösenordet måste innehålla minst en liten bokstav, en stor bokstav, en siffra och ett specialtecken.")]
        [Display(Name = "Nytt Lösenord")]
        public string NyttLosenord { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenord får inte vara tomt")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta Nytt Lösenord")]
        [Compare("NyttLosenord", ErrorMessage = "Det nya lösenordet matchar inte")]
        public string BekraftaLosenord { get; set; }
    }
}

