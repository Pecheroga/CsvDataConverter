using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using DataSource.Base;

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
        private SqlConnection _connection;

        public FromDb()
        {
            _programs = new ObservableCollection<Program>();
        }

        public void FillPrograms()
        {
            TryToSetConnectionString();
            using (_connection = new SqlConnection(DataSourceConnectionString))
            using (var adapter = new SqlDataAdapter("SELECT * FROM Programs", _connection))
            {
                var programs = new DataTable();
                adapter.Fill(programs);

                for (var i = 0; i < programs.Rows.Count; i++)
                {
                    var row = programs.Rows[i];
                    var program = new Program
                    {
                        Id = (int)row["Id"],
                        Title = row["Title"].ToString(),
                        StartLabel = row["StartLabel"].ToString(),
                        EndLabel = row["EndLabel"].ToString(),
                        Lang = row["Lang"].ToString(),
                        Author = row["Author"].ToString(),
                        Presenter = row["Presenter"].ToString()
                    };

                    _programs.Add(program);
                }
            }
        }

        public ObservableCollection<Program> GetPrograms()
        {
            return _programs;
        }

    }
}
