using AutoMapper;
using Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class PatientProfile : Profile
{
    public PatientProfile()
    {
        CreateMap<PatientCreateDto, Patient>();
        CreateMap<PatientUpdateDto, Patient>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
