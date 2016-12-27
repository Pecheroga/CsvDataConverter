namespace DataSource.Base
{
    public interface IOutputProgram
    {
        string StartTime { get; set; }
        string EndTime { get; set; }
        string Title { get; set; }
        string Lang { get; set; }
        string Author { get; set; }
        string Presenter { get; set; }
    }

    public class OutputProgram : IOutputProgram
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Title { get; set; }
        public string Lang { get; set; }
        public string Author { get; set; }
        public string Presenter { get; set; }
    }
}
