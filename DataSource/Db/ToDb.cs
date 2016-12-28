using System;
using System.Data;
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
        private SqlConnection _connection;

        public ToDb()
        {
            TryToSetConnectionString();
        }

        public void Add(Program program)
        {
            CheckFieldsLenthOf(program);

            const string query = "SET IDENTITY_INSERT Programs ON " +
                                 "INSERT INTO " +
                                 "Programs(Id, Title, StartLabel, EndLabel, Lang, Author, Presenter) " +
                                 "VALUES " +
                                 "(@Id, @Title, @StartLabel, @EndLabel, @Lang, @Author, @Presenter)";

            using (_connection = new SqlConnection(DataSourceConnectionString))
            using (var command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                SqlParameter[] parameters = {
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        Value = program.Id
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@Title",
                        Value = program.Title.Trim()
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@StartLabel",
                        Value = program.StartLabel.Trim()
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@EndLabel",
                        Value = program.EndLabel.Trim()
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@Lang",
                        Value = program.Lang.Trim()
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@Author",
                        Value = program.Author.Trim()
                    }, 
                    new SqlParameter
                    {
                        ParameterName = "@Presenter",
                        Value = program.Presenter.Trim()
                    } 
                };

                command.Parameters.AddRange(parameters);
                command.ExecuteNonQuery();
            }
        }

        public void Update(Program program)
        {
            CheckFieldsLenthOf(program);
            const string query = "UPDATE Programs SET " +
                                 "Title = @Title, " +
                                 "StartLabel = @StartLabel, " +
                                 "EndLabel = @EndLabel, " +
                                 "Lang = @Lang, " +
                                 "Author = @Author, " +
                                 "Presenter = @Presenter " +
                                 "WHERE Id = @Id";

            using (_connection = new SqlConnection(DataSourceConnectionString))
            using (var command = new SqlCommand(query, _connection))
            {
                _connection.Open();

                command.Parameters.AddWithValue("@Id", program.Id);
                command.Parameters.AddWithValue("@Title", program.Title);
                command.Parameters.AddWithValue("@StartLabel", program.StartLabel);
                command.Parameters.AddWithValue("@EndLabel", program.EndLabel);
                command.Parameters.AddWithValue("@Lang", program.Lang);
                command.Parameters.AddWithValue("@Author", program.Author);
                command.Parameters.AddWithValue("@Presenter", program.Presenter);

                command.ExecuteScalar();
            }
        }

        private void CheckFieldsLenthOf(Program program)
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

        public void Remove(Program program)
        {
            const string query = "DELETE FROM Programs WHERE Id = @Id";

            using (_connection = new SqlConnection(DataSourceConnectionString))
            using (var command = new SqlCommand(query, _connection))
            {
                _connection.Open();
                command.Parameters.AddWithValue("@Id", program.Id);

                command.ExecuteScalar();
            }
        }
    }
}
