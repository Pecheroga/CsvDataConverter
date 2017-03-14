namespace DataSource.Structure
{
    public interface IProgram : IProgramBase
    {
        int Id { get; set; }
        string StartLabel { get; set; }
        string EndLabel { get; set; }
    }

    public class Program : ProgramBase, IProgram
    {
        public int Id { get; set; }
        public string StartLabel { get; set; }
        public string EndLabel { get; set; }
    }
}
