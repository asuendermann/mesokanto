using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel;
using Commons.DomainModel.Attributes;
using Commons.DomainModel.Base;
using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Scrum {
    public class TeamMember : BaseTablePerHierarchy<int>, ITeamMember, IUniqueAuditable {

        public IEnumerable<ProjectTeamMember> TeamMemberProjects { get; set; }

        [MaxLength(DmConstants.MaxLength_8)]
        [Required]
        public string UserId { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        public string Name { get; set; }

        [MaxLength(DmConstants.MaxLength_1024)]
        [Required]
        [Email]
        public string Email { get; set; }

        public bool HasSameUniqueKey(object target) {
            var collaborator = target as TeamMember;
            if (null == collaborator) {
                return false;
            }

            var hasSameUniqueKey =
                0 == string.Compare(Email, collaborator.Email, StringComparison.OrdinalIgnoreCase);
            return hasSameUniqueKey;
        }
    }
}