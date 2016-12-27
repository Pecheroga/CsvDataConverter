namespace DataSource.Base
{
    public interface IProgram
    {
        int Id { get; set; }
        string Title { get; set; }
        string StartLabel { get; set; }
        string EndLabel { get; set; }
        string Lang { get; set; }
        string Author { get; set; }
        string Presenter { get; set; }
    }

    public class Program : IProgram
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string StartLabel { get; set; }
        public string EndLabel { get; set; }
        public string Lang { get; set; }
        public string Author { get; set; }
        public string Presenter { get; set; }
    }
}
