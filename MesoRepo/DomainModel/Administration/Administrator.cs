﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel;
using Commons.DomainModel.Attributes;
using Commons.DomainModel.Base;
using Commons.DomainModel.Domain;
using DomainModel.Base;

namespace DomainModel.Administration {
    public class Administrator : BaseTablePerHierarchy<int>, IUniqueAuditable, IAdministrator {
        public IEnumerable<ProjectAdministrator> AdministratorProjects { get; set; }

        [MaxLength(DmConstants.MaxLength_32)]
        [Required]
        public string UserIdentityName { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        [Required]
        public string Name { get; set; }

        [MaxLength(DmConstants.MaxLength_1024)]
        [Required]
        [Email]
        public string Email { get; set; }

        [MaxLength(DmConstants.MaxLength_64)]
        [Required]
        public string Phone { get; set; }

        public bool HasSameUniqueKey(object target) {
            var administrator = target as Administrator;
            if (null == administrator) return false;

            var sameUId = 0 == string.Compare(UserIdentityName, administrator.UserIdentityName,
                StringComparison.OrdinalIgnoreCase);
            return sameUId;
        }
    }
}