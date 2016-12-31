using System;
using System.Data.SqlClient;
using System.Linq;
using DataSource.Base;

namespace DataSource.Db
{
    public interface IToDb
    {
        void Update(Program program);
        void Remove(Program program);
        void Add(Program program);
    }

    public class ToDb : ConnectionString, IToDb
    {
        private Program _program;
        private string _query;

        public ToDb()
        {
            TryToGetConnectionString();
        }

        public void Add(Program program)
        {
            _program = program;
            CheckFieldsLenthOf(program);
            _query = "SET IDENTITY_INSERT Programs ON " +
                "INSERT INTO " +
                "Programs(Id, Title, StartLabel, EndLabel, Lang, Author, Presenter) " +
                "VALUES " +
                "(@Id, @Title, @StartLabel, @EndLabel, @Lang, @Author, @Presenter)";
            ExecuteNonQuery();
        }

        public void Update(Program program)
        {
            _program = program;
            CheckFieldsLenthOf(program);
            _query = "UPDATE Programs SET " +
                "Title = @Title, " +
                "StartLabel = @StartLabel, " +
                "EndLabel = @EndLabel, " +
                "Lang = @Lang, " +
                "Author = @Author, " +
                "Presenter = @Presenter " +
                "WHERE Id = @Id";
            ExecuteNonQuery();
        }

        public void Remove(Program program)
        {
            _program = program;
            _query = "DELETE FROM Programs WHERE Id = @Id";
            ExecuteNonQuery();
        }

        private void ExecuteNonQuery()
        {
            if (_query == null || _program == null) return;
            using (var connection = new SqlConnection(ProgramsDbConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(_query, connection))
                {
                    command.Parameters.AddWithValue("@Id", _program.Id);
                    command.Parameters.AddWithValue("@Title", _program.Title);
                    command.Parameters.AddWithValue("@StartLabel", _program.StartLabel);
                    command.Parameters.AddWithValue("@EndLabel", _program.EndLabel);
                    command.Parameters.AddWithValue("@Lang", _program.Lang);
                    command.Parameters.AddWithValue("@Author", _program.Author);
                    command.Parameters.AddWithValue("@Presenter", _program.Presenter);

                    command.ExecuteNonQuery();
                }
            }
        }

        private static void CheckFieldsLenthOf(Program program)
        {
            foreach (var propertyInfo in from propertyInfo in program.GetType().GetProperties()
                                         let propertyValue = propertyInfo.GetValue(program, null)
                                         as string
                                         where propertyValue != null && propertyValue.Length > 50
                                         select propertyInfo)
            {
                throw new Exception(string.Format("Lenth of the field : \"{0}\" must be less then 50 chars", propertyInfo));
            }
        }
    }
}
