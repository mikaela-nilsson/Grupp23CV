using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class UserSearchViewModel
    {
        [Required(ErrorMessage = "Du måste ange ett sökord.")]
        [RegularExpression(@"^[a-zA-ZåäöÅÄÖ\s]+$", ErrorMessage = "Sökningen får endast innehålla bokstäver och mellanslag.")]
        public string SearchQuery { get; set; }

        public List<CV> Results { get; set; } = new List<CV>();
        public bool SearchPerformed { get; set; }
    }
}

