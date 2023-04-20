using AutoMapper;
using JobOdysseyApi.Dtos;
using JobOdysseyApi.Models;

namespace JobOdysseyApi.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<ApplicationUser, AuthResponseDto>().ConstructUsing((user, context) => new AuthResponseDto{
            DisplayName = user.DisplayName!,
            Email = user.Email!,
            Token = context.Items["jwtToken"] as string ?? string.Empty,
            RefreshToken = context.Items["refreshToken"] as string ?? string.Empty
        });
        CreateMap<JobApplicationBoard, JobBoardDto>();
        CreateMap<JobApplicationBoard, PopulatedJobBoardDto>();
        CreateMap<JobApplication, JobApplicationDto>()
            .ForMember(dest => dest.JobBoardId, opt => opt.MapFrom(src => src.JobBoard.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}