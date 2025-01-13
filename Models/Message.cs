using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp23_CV.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Avsändarnamn krävs.")]
        public string SenderName { get; set; }

        [Display(Name = "Avsändare")]
        public int? SenderId { get; set; } //Nullable ifall man inte är inloggad

        [Required]
        public int ReceiverId { get; set; }
        [ForeignKey("ReceiverId")]
        public virtual User? Receiver { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual User? Sender { get; set; } //Kan vara null om man inte är inloggad


        [Display(Name = "Datum skickat")]
        public DateTime SentDate { get; set; } // När meddelandet skickades

        public bool IsRead { get; set; }

        [Required(ErrorMessage = "Meddelandeinnehåll krävs")]
        [Display(Name = "Meddelande")]
        public string Content { get; set; }
    }
}
