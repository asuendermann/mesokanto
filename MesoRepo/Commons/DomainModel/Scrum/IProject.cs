namespace Commons.DomainModel.Scrum {
    public interface IProject {
        public string Identifier { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}