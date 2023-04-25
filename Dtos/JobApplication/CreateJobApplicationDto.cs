using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JobOdysseyApi.Validations;

namespace JobOdysseyApi.Dtos;

/*
    NOTES:
    If this is done
    [Required]
    [ExtraValidationsHere]
    public DateTime ApplicationDate { get; set; }

                    OR
    [Required]
    [ExtraValidationsHere]            
    public int Status { get; set; }

    Both data annotations attibutes are ignored and a user 
    is able to send empty status or ApplicationDate.

    Thats why nullable types are used for Status or ApplicationDate
    with extra config to have some sort or input validation.
*/
public class CreateJobApplicationDto 
{
    [Required(ErrorMessage = "Job title field is required")]
    [MaxLength(50, ErrorMessage = "The job title field must have a maximum length of 50 characters.")]
    public string JobTitle { get; set; } = string.Empty;
    [Required(ErrorMessage = "Company name field is required")]
    [MaxLength(50, ErrorMessage = "The company name field must have a maximum length of 50 characters.")]
    public string CompanyName { get; set; } = string.Empty;
    [JsonConverter(typeof(NullableDateTimeConverter))]
    [ApplicationDateValidation(allowNull: false)]
    public DateTime? ApplicationDate { get; set; } = default;
    [MaxLength(2000, ErrorMessage = "The job description field must have a maximum length of 2000 characters.")]
    public string JobDescription { get; set; } = string.Empty; 
    public string JobUrl { get; set; } = string.Empty;
    [MaxLength(2000, ErrorMessage = "The notes field must have a maximum length of 2000 characters.")]
    public string Notes { get; set; } = string.Empty;
    [JsonConverter(typeof(NullableIntConverter))]
    [ApplicationStatusValidation(allowNull: false)]
    public int? Status { get; set; } = default;
    [Required(ErrorMessage = "Job board Id field is required")]
    public string JobBoardId { get; set; } = string.Empty;
}