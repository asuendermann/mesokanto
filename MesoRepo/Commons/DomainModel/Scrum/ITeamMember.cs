namespace Commons.DomainModel.Scrum {
    public interface ITeamMember {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }
    }
}