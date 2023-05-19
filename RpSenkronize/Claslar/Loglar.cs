using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpSenkronize.Claslar
{
  public  class Loglar
    {
        public void log_olustur(string mesaj, string duzey = "BILGI")
        {
            //  File.AppendAllText(Application.StartupPath + "\\getir.txt", string.Format("{0} [{1}]- {2}{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), duzey, mesaj, Environment.NewLine));
            File.AppendAllText(Application.StartupPath + "\\log.txt", string.Format("{0} [{1}]- {2} (PID: {3}){4}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), duzey, mesaj, Process.GetCurrentProcess().Id, Environment.NewLine));
        }
    }
}
