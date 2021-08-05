namespace Commons.DomainModel.Scrum {
    public interface IProjectOwner {
        public int ProjectId { get; set; }

        public int OwnerId { get; set; }
    }
}