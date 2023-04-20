using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Dtos;

public class CreateJobBoardDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}