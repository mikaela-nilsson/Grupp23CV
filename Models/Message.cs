using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; } // Unik identifierare för meddelandet

        [Required]
        [Display(Name = "Avsändare")]
        public string Sender { get; set; }

        public string Receiver { get; set; } // Har ej med Display Name, för om Recevier endast används internt i backend, för att indentifera vilken användare meddelandet skickats till, är det ej nödvändigt med Display Name
                    

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
