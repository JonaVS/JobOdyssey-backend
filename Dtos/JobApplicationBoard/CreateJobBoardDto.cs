using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Dtos;

public class CreateJobBoardDto
{
    [Required]
    [MaxLength(50, ErrorMessage = "The Name field must have a maximum length of 50 characters.")]
    public string Name { get; set; } = string.Empty;
}