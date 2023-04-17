using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Models;

public class JobApplicationBoard : TimeTrackableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public ApplicationUser User { get; set; } = new ApplicationUser();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
}