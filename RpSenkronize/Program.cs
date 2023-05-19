using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpSenkronize
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        public string gelen_baslangic, gelen_bitis;

        Claslar.Subeler sub = new Claslar.Subeler();
        Claslar.Senkronize senk = new Claslar.Senkronize();
        Claslar.Loglar loglar = new Claslar.Loglar();
        static void Main(string[] args)
        {
            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 0);
            Program program = new Program();
            if (Process.GetProcessesByName(Assembly.GetEntryAssembly().GetName().Name).Count() > 1)
            {

                program.loglar.log_olustur("Birden fazla açılmaya çalıştı");
                //  MessageBox.Show("Çalışıyor...", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                //Application.Exit();
            }
            else
            {
             
                program.baslat();
                program.istegi_dinle();
                program.loglar.log_olustur("Başlatıldı");
                Console.ReadKey();

            }

        

        }
        public void baslat()
        {
            sub.connectionlar();
            senk.genel_toplam_bilgileri(DateTime.Now.ToString(), DateTime.Now.ToString());
        }
        public class degerler
        {

            public string baslangic { get; set; }
            public string bitis { get; set; }

        }
        public void istegi_dinle()
        {
            try
            {
                

                HttpListener listener = new HttpListener();
                // Dinlenecek URL'yi belirt
                // listener.Prefixes.Add("http://192.168.14.150/FastChef/rp/rp_tarihler/");
                listener.Prefixes.Add(ConfigurationManager.AppSettings["FastChefYolu"]);
                // HttpListener'i başlat
                listener.Start();
                Task.Run(() =>
                {
                    while (true)
                    {

                        // Gelen istekleri bekleyerek dinle
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;




                        Stream bodyStream = request.InputStream;
                        StreamReader reader = new StreamReader(bodyStream);
                        string requestBody = reader.ReadToEnd();


                        degerler deserializedObject = JsonConvert.DeserializeObject<degerler>(requestBody);
                        string baslangic = deserializedObject.baslangic;
                        string bitis = deserializedObject.bitis;
                        // Gelen veriyi oku

                        // İstek üzerinde işlemler yapabilirsiniz

                        // İsteğe cevap gönder (isteğe bağlı)


                        DateTime dt = Convert.ToDateTime(baslangic);
                        gelen_baslangic = dt.ToString("yyyy-MM-dd 03:59:00");

                        DateTime dt2 = DateTime.ParseExact(bitis, "dd/MM/yyyy", null);
                        gelen_bitis = dt2.AddDays(1).ToString("yyyy-MM-dd 03:59:59");


                        sub.connectionlar();
                        senk.genel_toplam_bilgileri(gelen_baslangic, gelen_bitis);


                        HttpListenerResponse response = context.Response;
                        string responseString = "Merhaba! HTTP isteğiniz alındı.";
                        byte[] responseBytes = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentType = "text/plain";
                        response.ContentLength64 = responseBytes.Length;
                        Stream outputStream = response.OutputStream;
                        outputStream.Write(responseBytes, 0, responseBytes.Length);
                        outputStream.Close();

                    }
                });
            }
            catch (Exception ex)
            {
                loglar.log_olustur("HttpListener Hatası" + ex);
              //  MessageBox.Show(ex.ToString());

            }

        }


    }
}
