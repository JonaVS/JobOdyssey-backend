using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace JobOdysseyApi.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(50)]
    public string? DisplayName { get; set; }
}