using System;
using System.Reflection;

using AutoMapper;

namespace HelloCoreBll.BusinesLayerLogic {
    public abstract class AbstractBllManager {
        protected AbstractBllManager(IServiceProvider provider) {
            var config = new MapperConfiguration(cfg => { cfg.AddMaps(Assembly.GetExecutingAssembly()); });
            Mapper = config.CreateMapper();
        }

        protected IMapper Mapper { get; set; }
    }
}