namespace DataSource.Structure
{
    public interface IOutputProgram : IProgramBase
    {
        string StartTime { get; set; }
        string EndTime { get; set; }
    }

    public class OutputProgram : ProgramBase, IOutputProgram
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
