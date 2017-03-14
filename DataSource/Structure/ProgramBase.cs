namespace DataSource.Structure
{
    public interface IProgramBase
    {
        string Title { get; set; }
        string Subject { get; set; }
        string Lang { get; set; }
        string Author { get; set; }
        string Presenter { get; set; }
    }

    public class ProgramBase
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Lang { get; set; }
        public string Author { get; set; }
        public string Presenter { get; set; }
    }
}
