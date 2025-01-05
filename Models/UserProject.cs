using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Grupp23_CV.Models
{

    public class UserProject
    {
        public int UserId { get; set; } // Främmande nyckel till User
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
       
        public int ProjectId { get; set; } // Främmande nyckel till Project
        [ForeignKey(nameof(ProjectId))]

        public virtual Project Project { get; set; }

    }

}

