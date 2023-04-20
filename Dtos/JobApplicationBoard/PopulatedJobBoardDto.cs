namespace JobOdysseyApi.Dtos;

public class PopulatedJobBoardDto : JobBoardResponseDto
{
    public List<JobApplicationDto> JobApplications { get; set; } = new List<JobApplicationDto>();
} 