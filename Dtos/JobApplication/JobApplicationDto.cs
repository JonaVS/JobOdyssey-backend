namespace JobOdysseyApi.Dtos;

public class JobApplicationDto 
{
    public string Id { get; set; } = string.Empty;
    public string JobBoardId { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public DateTimeOffset ApplicationDate { get; set; }
    public string JobDescription { get; set; } = string.Empty;
    public string JobUrl { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}