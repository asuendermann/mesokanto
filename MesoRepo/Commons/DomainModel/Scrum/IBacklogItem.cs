namespace Commons.DomainModel.Scrum {
    public interface IBacklogItem {
        public int ProjectOwnerId { get; set; }

        public int ProjectId { get; set; }

        public string Header { get; set; }

        public string Content { get; set; }
    }
}