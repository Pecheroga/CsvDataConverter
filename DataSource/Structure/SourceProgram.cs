namespace DataSource.Structure
{
    public interface ISourceProgram
    {
        double SourceStartTime { get; set; }
        string SourceTitle { get; set; }
        double SourceDuration { get; set; }
    }

    public class SourceProgram : ISourceProgram
    {
        public double SourceStartTime { get; set; }
        public string SourceTitle { get; set; }
        public double SourceDuration { get; set; }
    }
}
