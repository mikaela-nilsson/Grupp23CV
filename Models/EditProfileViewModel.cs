using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Grupp23_CV.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Fullständigt namn är obligatoriskt.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Adress är obligatorisk.")]
        public string Adress { get; set; }

        [Phone(ErrorMessage = "Ange ett giltigt telefonnummer.")]
        public string PhoneNumber { get; set; }

        

        public bool IsPrivate { get; set; } // Profilstatus

        public List<SelectListItem> IsPrivateOptions { get; set; } = new()
        {
            new SelectListItem { Value = "true", Text = "Privat" },
            new SelectListItem { Value = "false", Text = "Offentlig" }
        };
    }
}

