using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GalleryWeb.Models
{
    public class IKSDAL
    {

        private string constr;

        private SqlConnection _Connection = null;
        private SqlTransaction _Transaction = null;
        private bool _AutoCloseConnection = false;
        private SqlException _Exception = null;
        private string _Message = string.Empty;

        public IKSDAL(string connectionstring)
        {
            constr = connectionstring;
        }

        public SqlConnection Connection
        {
            get { return _Connection; }
            set { _Connection = value; }
        }
        public SqlTransaction Transaction
        {
            get { return _Transaction; }
            set { _Transaction = value; }
        }
        public bool AutoCloseConnection
        {
            get { return _AutoCloseConnection; }
            set { _AutoCloseConnection = value; }
        }
        public SqlException Exception
        {
            get { return _Exception; }
            set { _Exception = value; }
        }
        public string Message
        {
            get { return _Message; }
            set { _Message = value; }
        }


        //DAL.Connection.BeginTransaction("CRUDTransaction");
        //DAL.Transaction.Commit();
        //DAL.Transaction.Rollback();

        //string conn_str = "Server=SERVERIP;Database=DATABASENAME;User Id=DBUSER;Password=DBPW;";
        //DAL.Connection = new SqlConnection(@constr);

        //Sample:
        //
        //using System.Collections.Generic;
        //using System.Data;
        //using System.Data.SqlClient;
        //using IKS.DAL;
        //
        //var DAL = new IKSDAL(sConnection);
        //DataTable dt = new DataTable();
        //List<SqlParameter> parameter = new List<SqlParameter>();
        //
        //string sql = "SELECT [NOMORPOLISI] FROM [timbangan].[dbo].[tMasterParkir] WHERE NOMORPOLISI=@nopol";
        //
        //parameter.Add(new SqlParameter() { ParameterName = "@nopol", SqlDbType = SqlDbType.VarChar, Value = "B9457P0" } );
        //
        //dt = DAL.GetDataTable(sql, parameter);

        public DataTable GetDataTable(string sql, List<SqlParameter> param = null)
        {
            try
            {
                DataTable dt = new DataTable();

                if (Connection == null) Connection = new SqlConnection(@constr);
                if (Connection.State != ConnectionState.Open) Connection.Open();

                using (SqlConnection con = Connection)
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;

                        if (param != null)
                            cmd.Parameters.AddRange(param.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dt);
                        }
                    }
                }

                if (AutoCloseConnection && Transaction == null && Connection.State == ConnectionState.Open) Connection.Close();

                return dt;

            }
            catch (SqlException ex)
            {
                this.Message = ex.Message;
                this.Exception = ex;
                return null;
            }
        }

        //Sample:
        //
        //using System.Collections.Generic;
        //using System.Data;
        //using System.Data.SqlClient;
        //using IKS.DAL;
        //
        //var DAL = new IKSDAL(sConnection);
        //DataTable dt = new DataTable();
        //List<SqlParameter> parameter = new List<SqlParameter>();
        //
        //EXEC sp_PrintNetto_SOT '2017-06-05','2017-06-06' 
        //parameter.Add(new SqlParameter() { ParameterName = "@date1", SqlDbType = SqlDbType.VarChar, Value = "2017-06-05" } );
        //parameter.Add(new SqlParameter() { ParameterName = "@date2", SqlDbType = SqlDbType.VarChar, Value = "2017-06-06" } );
        //
        //dt = DAL.GetSPDataTable("sp_PrintNetto_SOT", parameter);

        public DataTable GetSPDataTable(string spname, List<SqlParameter> param = null)
        {
            try
            {
                DataTable dt = new DataTable();

                if (Connection == null) Connection = new SqlConnection(@constr);
                if (Connection.State != ConnectionState.Open) Connection.Open();

                using (SqlConnection con = Connection)
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = spname;
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (param != null)
                            cmd.Parameters.AddRange(param.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(dt);
                        }
                    }
                }

                if (AutoCloseConnection && Transaction == null && Connection.State == ConnectionState.Open) Connection.Close();

                return dt;

            }
            catch (SqlException ex)
            {
                this.Message = ex.Message;
                this.Exception = ex;
                return null;
            }
        }

        //Sample:
        //
        //using System.Collections.Generic;
        //using System.Data;
        //using System.Data.SqlClient;
        //using IKS.DAL;
        //
        //var DAL = new IKSDAL(sConnection);
        //DataSet ds = new DataSet();
        //List<SqlParameter> parameter = new List<SqlParameter>();
        //
        ////EXEC [sp_Monitoring_Step_v1] '2017-06-04','2017-06-06','ALL','A' 
        //parameter.Add(new SqlParameter() { ParameterName = "@dtFROM", SqlDbType = SqlDbType.VarChar, Value = "2017-06-04" } );
        //parameter.Add(new SqlParameter() { ParameterName = "@dtTo", SqlDbType = SqlDbType.VarChar, Value = "2017-06-06" } );
        //parameter.Add(new SqlParameter() { ParameterName = "@Gudang", SqlDbType = SqlDbType.VarChar, Value = "ALL" } );
        //parameter.Add(new SqlParameter() { ParameterName = "@LE", SqlDbType = SqlDbType.VarChar, Value = "A" } );

        //ds = DAL.GetSPDataSet("sp_Monitoring_Step_v1", parameter);

        public DataSet GetSPDataSet(string spname, List<SqlParameter> param = null)
        {
            try
            {
                DataSet ds = new DataSet();

                if (Connection == null) Connection = new SqlConnection(@constr);
                if (Connection.State != ConnectionState.Open) Connection.Open();

                using (SqlConnection con = Connection)
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = spname;
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (param != null)
                            cmd.Parameters.AddRange(param.ToArray());

                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = cmd;
                            da.Fill(ds);
                        }
                    }
                }

                if (AutoCloseConnection && Transaction == null && Connection.State == ConnectionState.Open) Connection.Close();

                return ds;

            }
            catch (SqlException ex)
            {
                this.Message = ex.Message;
                this.Exception = ex;
                return null;
            }
        }

        public void ExecuteNonQuery(string sql, List<SqlParameter> param = null)
        {
            try
            {
                if (Connection == null) Connection = new SqlConnection(@constr);
                if (Connection.State != ConnectionState.Open) Connection.Open();

                SqlCommand cmd = Connection.CreateCommand();

                if (Transaction != null) cmd.Transaction = Transaction;

                cmd.CommandTimeout = 0;
                cmd.CommandText = sql;

                if (param != null)
                    cmd.Parameters.AddRange(param.ToArray());

                cmd.ExecuteNonQuery();

                if (AutoCloseConnection && Transaction == null && Connection.State == ConnectionState.Open) Connection.Close();

            }
            catch (SqlException ex)
            {
                this.Message = ex.Message;
                this.Exception = ex;
            }
        }

        //Sample:
        //
        //using System.Collections.Generic;
        //using System.Data;
        //using System.Data.SqlClient;
        //using IKS.DAL;
        //
        //var DAL = new IKSDAL(sConnection);
        //List<SqlParameter> parameter = new List<SqlParameter>();
        //int retvalue;
        //
        //string sql = "SELECT COUNT([NOMORPOLISI]) FROM [timbangan].[dbo].[tMasterParkir] WHERE NOMORPOLISI=@nopol";
        //
        //parameter.Add(new SqlParameter() { ParameterName = "@nopol", SqlDbType = SqlDbType.VarChar, Value = "Z9898HD" } );
        //
        //retvalue = (int) DAL.ExecuteScalar(sql, parameter);

        public Object ExecuteScalar(string sql, List<SqlParameter> param = null)
        {
            try
            {
                object ret = null;

                if (Connection == null) Connection = new SqlConnection(@constr);
                if (Connection.State != ConnectionState.Open) Connection.Open();

                SqlCommand cmd = Connection.CreateCommand();

                if (Transaction != null) cmd.Transaction = Transaction;

                cmd.CommandTimeout = 0;
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;

                if (param != null)
                    cmd.Parameters.AddRange(param.ToArray());

                ret = cmd.ExecuteScalar();

                if (AutoCloseConnection && Transaction == null && Connection.State == ConnectionState.Open) Connection.Close();

                return ret;

            }
            catch (SqlException ex)
            {
                this.Message = ex.Message;
                this.Exception = ex;
                return null;
            }
        }
    }
}