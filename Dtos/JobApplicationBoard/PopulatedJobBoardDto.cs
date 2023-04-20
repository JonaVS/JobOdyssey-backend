namespace JobOdysseyApi.Dtos;

public class PopulatedJobBoardDto : JobBoardDto
{
    public List<JobApplicationDto> JobApplications { get; set; } = new List<JobApplicationDto>();
} 