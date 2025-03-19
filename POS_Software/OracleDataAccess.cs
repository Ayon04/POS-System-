using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;

namespace POS_Software
{
    public class OracleDataAccess
    {
        private readonly string connectionString = "User Id=db;Password=1234;Data Source=//localhost:1521/XE";
        public readonly OracleConnection connection;

        public OracleDataAccess()
        {
            this.connection = new OracleConnection(connectionString);
        }

        private void OpenConnection()
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        private void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public DataTable ExecuteQuery(string query)
        {
            OpenConnection();

            using (OracleCommand command = new OracleCommand(query, connection))
            {
                using (OracleDataAdapter adapter = new OracleDataAdapter(command))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }
        public int ExecuteNonQuery(string query)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        public void Close()
        {
            CloseConnection();
        }

        public object ExecuteScalar(string query)
        {
            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
