using System.Collections.ObjectModel;
using System.Data.SqlClient;
using DataSource.Structure;

namespace DataSource.Db
{
    public interface IFromDb
    {
        void FillPrograms();
        ObservableCollection<Program> GetPrograms();
    }

    public class FromDb : ConnectionString, IFromDb
    {
        private readonly ObservableCollection<Program> _programs;

        public FromDb()
        {
            _programs = new ObservableCollection<Program>();
        }

        public void FillPrograms()
        {
            TryToGetConnectionString();
            using (var connection = new SqlConnection(ProgramsDbConnectionString))
            {
                connection.Open();
                const string query = "SELECT * FROM Programs";
                using (var command = new SqlCommand(query, connection))
                using (var dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        var program = new Program
                        {
                            Id = (int)dataReader["Id"],
                            Title = dataReader["Title"].ToString(),
                            Subject = dataReader["Subject"].ToString(),
                            StartLabel = dataReader["StartLabel"].ToString(),
                            EndLabel = dataReader["EndLabel"].ToString(),
                            Lang = dataReader["Lang"].ToString(),
                            Author = dataReader["Author"].ToString(),
                            Presenter = dataReader["Presenter"].ToString()
                        };
                        _programs.Add(program);
                    }    
                }    
            }
        }

        public ObservableCollection<Program> GetPrograms()
        {
            return _programs;
        }
    }
}
