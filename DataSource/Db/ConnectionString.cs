using System;
using System.Configuration;

namespace DataSource.Db
{
    public class ConnectionString
    {
        public string DataSourceConnectionString;

        public void TryToSetConnectionString()
        {
            try
            {
                SetConnectionString();
            }
            catch (Exception)
            {
                throw new Exception("Can not create DataBase connection string.");
            }
        }

        private void SetConnectionString()
        {
            DataSourceConnectionString =
                ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }
    }
}
