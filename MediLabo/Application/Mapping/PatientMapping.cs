using AutoMapper;
using Domain.Entities;
namespace Application.Mapping;
public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<PatientCreateDto, Patient>();
        CreateMap<PatientUpdateDto, Patient>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
