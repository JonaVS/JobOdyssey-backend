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
            Token = context.Items["jwtToken"] as string ?? string.Empty
        });
    }
}