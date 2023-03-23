using AutoMapper;
using Dss.Domain.DTOs;
using Dss.Domain.Models;

public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientDto, Patient>();               
            CreateMap<Patient, PatientDto>();                
        }
    }