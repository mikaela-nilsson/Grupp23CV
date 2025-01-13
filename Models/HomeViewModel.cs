namespace Grupp23_CV.Models
{
    public class HomeViewModel
    {
        public List<CV> CVs { get; set; }
        public Project LatestProject { get; set; }

        public int CurrentUserId { get; set; }
    }
}
