using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetData.Data
{
    public class BaseClass
    {
        public SqlConnection _connection;  
        public BaseClass()
        {
            _connection = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Integrated Security=true;Initial Catalog=MyNet");
        }

        public DataTable ExecuteReader(string query)
        {
            DataTable ds = new DataTable();

            try
            {

                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                SqlCommand cmd = new SqlCommand(query, _connection);
                //using (SqlDataReader reader = cmd.ExecuteReader())
                //{
                //    if (reader.Read())
                //    {
                //        ds.Load(reader);
                //    }
                //}
                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                {
                    sda.Fill(ds);
                }
                _connection.Close();

            }
            catch (Exception e)
            {

            }
            return ds;
        }
        public void ExecuteNonQuery(string query)
        {
            try
            {

                SqlCommand cmd = new SqlCommand(query,_connection);
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                cmd.ExecuteNonQuery();
                _connection.Close();

            }
            catch (Exception e)
            {

            }
        }

        public SqlCommand GetProcedure(string procname)
        {
            SqlCommand cmd = new SqlCommand(procname, _connection);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public void ExecuteProcedure(SqlCommand sqlCommand)
        {
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            sqlCommand.ExecuteNonQuery();
            _connection.Close();
        }

        public DataSet ExecuteProcedureReader(SqlCommand sqlCommand)
        {
            DataSet dt = new DataSet();

            try
            {
                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();
                //sqlCommand.Connection.Open();
                using (SqlDataAdapter sda = new SqlDataAdapter(sqlCommand))
                {
                    sda.Fill(dt);
                }
                _connection.Close();

            }
            catch (Exception e)
            {

            }
            return dt;
        }
    }
}
