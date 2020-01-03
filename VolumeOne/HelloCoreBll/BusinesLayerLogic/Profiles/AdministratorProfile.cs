using AutoMapper;

using HelloCoreBll.DataTransferObjects;

using HelloCoreDal.DomainModel;

namespace HelloCoreBll.BusinesLayerLogic.Profiles {
    public class AdministratorProfile : Profile {
        public AdministratorProfile() {
            CreateMap<Administrator, AdministratorDto>();
            CreateMap<AdministratorDto, Administrator>();
            CreateMap<ProjectAdministrator, ProjectAdministratorDto>();
            CreateMap<ProjectAdministratorDto, ProjectAdministrator>();
            CreateMap<MasterAdministrator, MasterAdministratorDto>();
            CreateMap<MasterAdministratorDto, MasterAdministrator>();
        }
    }
}