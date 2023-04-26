using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using JobOdysseyApi.Validations;

namespace JobOdysseyApi.Dtos;

public class UpdateJobApplicationDto
{
    [MaxLength(50, ErrorMessage = "The job title field must have a maximum length of 50 characters.")]
    public string? JobTitle { get; set; }
    [MaxLength(50, ErrorMessage = "The company name field must have a maximum length of 50 characters.")]
    public string? CompanyName { get; set; }
    [JsonConverter(typeof(NullableDateTimeConverter))]
    [ApplicationDateValidation(allowNull: true)]
    public DateTime? ApplicationDate { get; set; }
    [MaxLength(2000, ErrorMessage = "The job description field must have a maximum length of 2000 characters.")]
    public string? JobDescription { get; set; }
    public string? JobUrl { get; set; }
    [MaxLength(2000, ErrorMessage = "The notes field must have a maximum length of 2000 characters.")]
    public string? Notes { get; set; }
}