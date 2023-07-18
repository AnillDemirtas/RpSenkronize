using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RpSenkronize.Claslar
{
    public class Subeler
    {
        Database db = new Database();
        SqlCommand komut = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        Claslar.Loglar loglar = new Claslar.Loglar();

        public SqlDataReader sube_bilgileri()
        {
            SqlDataReader dr;
            db.OpenConnection(Database.txt);
            komut = new SqlCommand("select * from Branch", db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader(CommandBehavior.CloseConnection);
            return dr;



        }
        public void connectionlar()
        {
            try
            {
                Degiskenler.connectionlar.Clear();
                var sube_bilgileri = this.sube_bilgileri();
                while (sube_bilgileri.Read())
                {
                    string txt_bilgisi = string.Format(@"Data Source={0};Initial Catalog=FASTCHEF;Persist Security Info=True; User ID=sa;Password=netkod12345**", sube_bilgileri["Sube_ip"]);
                    SqlConnection con = new SqlConnection(txt_bilgisi);
                    if (QuickOpen(con, 3000) == true)
                    {
                        Degiskenler.connectionlar.Add(txt_bilgisi);
                        Degiskenler.subeler.Add(int.Parse(sube_bilgileri["id"].ToString()));

                    }
                    else
                    {
                        Degiskenler.connectionlar.Add("Yok");
                        Degiskenler.subeler.Add(int.Parse(sube_bilgileri["id"].ToString()));
                    }


                }
            }
            catch (Exception ex)
            {
                loglar.log_olustur("connectionlar clasında hata" + ex);

            }
           
        }
        public bool QuickOpen(SqlConnection conn, int timeout)
        {
            // We'll use a Stopwatch here for simplicity. A comparison to a stored DateTime.Now value could also be used
            Stopwatch sw = new Stopwatch();
            bool connectSuccess = false;

            // Try to open the connection, if anything goes wrong, make sure we set connectSuccess = false
            Thread t = new Thread(delegate ()
            {
                try
                {
                    sw.Start();
                    conn.Open();
                    conn.Close();
                    connectSuccess = true;

                }
                catch { }
            });

            // Make sure it's marked as a background thread so it'll get cleaned up automatically
            t.IsBackground = true;
            t.Start();

            // Keep trying to join the thread until we either succeed or the timeout value has been exceeded
            while (timeout > sw.ElapsedMilliseconds)
                if (t.Join(1))
                    break;

            // If we didn't connect successfully, throw an exception
            if (!connectSuccess)
                return false;
            else
            {
                return true;
            }
            //throw new Exception("Timed out while trying to connect.");
        }


    }
}
