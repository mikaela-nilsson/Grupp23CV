using Grupp23_CV.Models;

namespace Grupp23_CV.Models;
public class CVProjectsViewModel
{
    public CV CV { get; set; }
    public List<Project> Projects { get; set; }

    public bool IsOwnCV { get; set; }
}
