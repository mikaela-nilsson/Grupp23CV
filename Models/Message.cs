using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; } // Unik identifierare för meddelandet

        [Required]
        [Display(Name = "Avsändare")]
        public int SenderId { get; set; }

        public int ReceiverId { get; set; }
                    

        [Required]
        [Display(Name = "Datum skickat")]
        public DateTime SentDate { get; set; } // När meddelandet skickades

        [Display(Name = "Läst meddelande")]
        public bool IsRead { get; set; }

        [Required]
        [Display(Name = "Meddelande")]
        public string Content { get; set; } // Själva meddelandetexten
    }
}
