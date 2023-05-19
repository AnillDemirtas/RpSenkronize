using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpSenkronize.Claslar
{
    public class Database
    {
        public static string txt = System.Configuration.ConfigurationManager.ConnectionStrings["FASTCHEF"].ConnectionString;
        public SqlConnection cn = null;
        bool netgit = false, sqlgit = false;
        public bool OpenConnection(string gelen_txt)
        {

            if (sqlgit == false)
            {
                try
                {
                    SqlConnection.ClearAllPools();
                    cn = new SqlConnection(gelen_txt);
                    if (cn.State == System.Data.ConnectionState.Closed)
                    {
                        cn.Open();
                    }
                    else
                    {
                        cn.Close();
                    }
                }
                catch (Exception ex)
                {

                    //object a = ex.Message;
                    //CloseConnection();
                }
            }
            return sqlgit;

        }

        public void CloseConnection()
        {
            try
            {
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }
            catch (Exception)
            {

                CloseConnection();
            }
        }
    }
}
