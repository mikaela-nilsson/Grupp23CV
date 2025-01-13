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
        [Display(Name = "Nytt Lösenord")]
        public string NyttLosenord { get; set; }

        [Required(ErrorMessage = "Bekräfta lösenord får inte vara tomt")]
        [DataType(DataType.Password)]
        [Display(Name = "Bekräfta Nytt Lösenord")]
        [Compare("NyttLosenord", ErrorMessage = "Det nya lösenordet matchar inte")]
        public string BekraftaLosenord { get; set; }
    }
}

