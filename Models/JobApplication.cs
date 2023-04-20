using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Models;

public enum JobApplicationStatus 
{
  Applied,
  Interviewing,
  Offer,
  Rejected,
  Ghosted,
  Withdrawn
}

public class JobApplication : TimeTrackableEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required]
    public string JobTitle { get; set; } = string.Empty;
    [Required]
    public string CompanyName { get; set; } = string.Empty;
    [Required]
    public DateTimeOffset ApplicationDate { get; set; }
    public string? JobDescription { get; set; }
    public string? JobUrl { get; set; }
    public string? Notes { get; set; }
    [Required]
    public JobApplicationStatus Status { get; set; }
    [Required]
    public ApplicationUser User { get; set; } = new ApplicationUser();
    public JobApplicationBoard JobBoard { get; set; } = new JobApplicationBoard();
}