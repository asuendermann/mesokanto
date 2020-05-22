using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Commons.DomainModel;
using Commons.DomainModel.Base;
using Commons.DomainModel.Scrum;
using DomainModel.Base;

namespace DomainModel.Domain {
    public class Project : BaseAuditable<int>, IProject, IUniqueAuditable {
        public IEnumerable<ProjectTeamMember> ProjectTeamMembers { get; set; }

        public IEnumerable<ProjectScrumMaster> ProjectScrumMasters { get; set; }

        public IEnumerable<ProjectOwner> ProjectOwners { get; set; }

        public IEnumerable<BacklogItem> BacklogItems { get; set; }

        [MaxLength(DmConstants.MaxLength_16)]
        [Required]
        public string Identifier { get; set; }

        [MaxLength(DmConstants.MaxLength_256)]
        [Required]
        public string Title { get; set; }

        [MaxLength]
        public string Description { get; set; }

        public bool HasSameUniqueKey(object target) {
            var project = target as Project;
            if (null == project) {
                return false;
            }

            var hasSameUniqueKey =
                0 == string.Compare(Identifier, project.Identifier, StringComparison.OrdinalIgnoreCase);
            return hasSameUniqueKey;
        }
    }
}