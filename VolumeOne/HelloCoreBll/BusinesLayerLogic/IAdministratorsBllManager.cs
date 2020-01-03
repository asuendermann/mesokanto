using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using HelloCoreBll.DataTransferObjects;

namespace HelloCoreBll.BusinesLayerLogic {
    public interface IAdministratorsBllManager {
        bool IsAdmin(string userIdentityName);

        bool IsMaster(string userIdentityName);

        int CountAdministrators();

        int CountMasterAdministrators();

        int CountProjectAdministrators();

        IEnumerable<MasterAdministratorDto> ReadMasterAdministrators
            ( params Expression<Func<MasterAdministratorDto, object>>[] paths);
    }
}