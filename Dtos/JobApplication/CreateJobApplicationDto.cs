using System.ComponentModel.DataAnnotations;
using JobOdysseyApi.Validations;

namespace JobOdysseyApi.Dtos;

public class CreateJobApplicationDto 
{
    [Required(ErrorMessage = "Job title field is required")]
    [MaxLength(50, ErrorMessage = "The job title field must have a maximum length of 50 characters.")]
    public string JobTitle { get; set; } = string.Empty;
    [Required(ErrorMessage = "Company name field is required")]
    [MaxLength(50, ErrorMessage = "The company name field must have a maximum length of 50 characters.")]
    public string CompanyName { get; set; } = string.Empty;
    [ApplicationDateValidation]
    public DateTime? ApplicationDate { get; set; } = default;
    [MaxLength(2000, ErrorMessage = "The job description field must have a maximum length of 2000 characters.")]
    public string JobDescription { get; set; } = string.Empty; 
    public string JobUrl { get; set; } = string.Empty;
    [MaxLength(2000, ErrorMessage = "The notes field must have a maximum length of 2000 characters.")]
    public string Notes { get; set; } = string.Empty;
    [Required(ErrorMessage = "Status field is required")]
    [ApplicationStatusValidation]
    public int? Status { get; set; } = default;
    [Required]
    public string JobBoardId { get; set; } = string.Empty;
}