using System;
using System.Reflection;
using AutoMapper;
using Commons.Cryptography;
using DatabaseAccess;
using DataTransferObjects.Base;
using DomainModel.Base;
using Microsoft.Extensions.DependencyInjection;

namespace BusinesLogic {
    public class BaseBusinessLogic : IDisposable {
        protected readonly BaseDbContext Context;

        protected readonly ICryptoService<int> CryptoService;

        protected readonly IMapper Mapper;

        protected BaseBusinessLogic(IServiceProvider provider) {
            CryptoService = provider.GetService<ICryptoService<int>>();
            var configuration = new MapperConfiguration(Init());
            Mapper = configuration.CreateMapper();
            Context = provider.GetService<BaseDbContext>();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) { }
        }

        public Action<IMapperConfigurationExpression> Init() {
            return cfg => {
                cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.CreateMap<BaseAuditable<int>, BaseAuditableDto<int>>()
                    .ForMember(dest => dest.Reference,
                        opt => opt.MapFrom(src => CryptoService.Encrypt(src.Id)))
                    .IncludeAllDerived();
                cfg.CreateMap<BaseAuditableDto<int>, BaseAuditable<int>>()
                    .ForMember(dest => dest.Id,
                        opt => opt.MapFrom(src => CryptoService.Decrypt(src.Reference)))
                    .IncludeAllDerived();
            };
        }
    }
}