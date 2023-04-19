using System.ComponentModel.DataAnnotations;

namespace JobOdysseyApi.Dtos;

public class JobBoardCreateRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}