using System.ComponentModel.DataAnnotations;

namespace Grupp23_CV.Models
{

    public class UserProject
    {
        public int UserId { get; set; } // Främmande nyckel till User
        
       
        public int ProjectId { get; set; } // Främmande nyckel till Project


    }

}

