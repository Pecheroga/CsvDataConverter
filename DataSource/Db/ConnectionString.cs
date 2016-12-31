using System;
using System.Configuration;

namespace DataSource.Db
{
    public class ConnectionString
    {
        public string ProgramsDbConnectionString;

        public void TryToGetConnectionString()
        {
            try
            {
                GetConnectionString();
            }
            catch (Exception)
            {
                throw new Exception("Can not create DataBase connection string.");
            }
        }

        private void GetConnectionString()
        {
            ProgramsDbConnectionString =
                ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }
    }
}
