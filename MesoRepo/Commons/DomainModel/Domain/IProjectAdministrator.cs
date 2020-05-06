using System;
using System.Collections.Generic;
using System.Text;

namespace Commons.DomainModel.Domain {
    public interface IProjectAdministrator {
        public int ProjectId { get; set; }

        public int AdministratorId { get; set; }
    }
}
