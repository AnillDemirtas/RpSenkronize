using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpSenkronize.Claslar
{
    public class SqlKayit
    {
        Database dd = new Database();
        SqlCommand komut = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();

        public void genel_toplam_kayit(int sube_id, decimal toplamtutar, decimal iade, decimal iskonto, decimal iptal, bool baglanti_durumu)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"BEGIN TRAN
                                    IF EXISTS (
                                      SELECT * 
                                      FROM Rp_toplamlar WITH (UPDLOCK, SERIALIZABLE) 
                                      WHERE sube_id = '{0}'
                                    ) 
                                    BEGIN 
                                      UPDATE Rp_toplamlar 
                                      SET tutar = '{1}',iade = '{2}',iskonto = '{3}',iptal = '{4}',baglanti_durumu='{5}'  where sube_id = {0}
                                    END 
                                    ELSE 
                                    BEGIN 
                                      INSERT INTO Rp_toplamlar (sube_id,tutar,iade,iskonto,iptal,baglanti_durumu) 
                                      OUTPUT inserted.id 
                                      VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')
                                    END 
                                    COMMIT TRAN;", sube_id, toplamtutar, iade, iskonto, iptal, baglanti_durumu), dd.cn);
            komut.Parameters.AddWithValue("@baglanti_durumu", baglanti_durumu);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void subelerin_cok_satilan(int sube_id, string urun_adi, int sayi, bool baglanti_durumu)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"BEGIN TRAN
                                    IF EXISTS (
                                      SELECT * 
                                      FROM RpSubelerinCokSatilan WITH (UPDLOCK, SERIALIZABLE) 
                                      WHERE sube_id = '{0}'
                                    ) 
                                    BEGIN 
                                      UPDATE RpSubelerinCokSatilan 
                                      SET urun_adi = @urun_adi,sayi = '{2}', baglanti_durumu='{3}'  where sube_id = {0}
                                    END 
                                    ELSE 
                                    BEGIN 
                                      INSERT INTO RpSubelerinCokSatilan (sube_id,urun_adi,sayi,baglanti_durumu) 
                                      OUTPUT inserted.id 
                                      VALUES ('{0}',@urun_adi,'{2}','{3}')
                                    END 
                                    COMMIT TRAN;", sube_id, urun_adi, sayi, baglanti_durumu), dd.cn);
            komut.Parameters.AddWithValue("@baglanti_durumu", baglanti_durumu);
            komut.Parameters.Add(@"urun_adi", urun_adi);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void subelerin_cok_satilan_sil()
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"truncate table RpSubelerinCokSatilan"), dd.cn);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void satilan_urun_detaylarini_sil()
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"truncate table RpSubelerinSatilanUrunDetaylari"), dd.cn);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }

        public void genel_toplamlari_sifirla()
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"update Rp_toplamlar set tutar='0',iade='0',iskonto='0',iptal='0'"), dd.cn);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void acik_masa_tutari(int sube_id, decimal toplam, int sayi, bool baglanti_durumu)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"BEGIN TRAN
                                    IF EXISTS (
                                      SELECT * 
                                      FROM RpAcikMasalar WITH (UPDLOCK, SERIALIZABLE) 
                                      WHERE sube_id = '{0}'
                                    ) 
                                    BEGIN 
                                      UPDATE RpAcikMasalar 
                                      SET toplam = @toplam,sayi = '{2}', baglanti_durumu='{3}'  where sube_id = {0}
                                    END 
                                    ELSE 
                                    BEGIN 
                                      INSERT INTO RpAcikMasalar (sube_id,toplam,sayi,baglanti_durumu) 
                                      OUTPUT inserted.id 
                                      VALUES ('{0}',@toplam,'{2}','{3}')
                                    END 
                                    COMMIT TRAN;", sube_id, toplam, sayi, baglanti_durumu), dd.cn);
            komut.Parameters.AddWithValue("@baglanti_durumu", baglanti_durumu);
            komut.Parameters.Add(@"toplam", toplam);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void acik_masa_tutari_sil(int sube_id)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"delete from RpAcikMasalar where sube_id='" + sube_id + "' ", sube_id), dd.cn);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
        }
        public void subelerin_satilanlari(int sube_id, string urun_adi, int urun_id, decimal sayi, decimal toplam, bool baglanti_durumu)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@" 
                                      INSERT INTO RpSubelerinSatilanlari (sube_id,urun_adi,urun_id,toplam_sayi,toplam_tutar,baglanti_durumu) 
                                   
                                      VALUES ('{0}',@urun_adi,'{2}','{3}',{4},'{5}') ", sube_id, urun_adi, urun_id, toplam, sayi, baglanti_durumu), dd.cn);
            komut.Parameters.AddWithValue("@baglanti_durumu", baglanti_durumu);
            komut.Parameters.Add(@"urun_adi", urun_adi);
            komut.ExecuteNonQuery();
            dd.CloseConnection();

        }
        public void satilan_urun_detaylari(int sube_id, int urun_id, string urun_adi, decimal sayi, decimal toplam, bool baglanti_durumu)
        {
            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"
                                      INSERT INTO RpSubelerinSatilanUrunDetaylari (sube_id,urun_id,urun_adi,toplam_sayi,toplam_tutar,baglanti_durumu) 
                                      OUTPUT inserted.id 
                                      VALUES ({0},'{1}',@urun_adi,'{3}',{4},'{5}')
                                  ", sube_id, urun_id, urun_adi, sayi, toplam, baglanti_durumu), dd.cn);
            komut.Parameters.AddWithValue("@baglanti_durumu", baglanti_durumu);
            komut.Parameters.Add(@"urun_adi", urun_adi);
            komut.ExecuteNonQuery();
            dd.CloseConnection();

        }
        public void subelerde_onceden_kalan_satilmayan_stoklari_sil()
        {

            dd.OpenConnection(Database.txt);
            komut = new SqlCommand(string.Format(@"truncate table RpSubelerinSatilanlari"), dd.cn);
            komut.ExecuteNonQuery();
            dd.CloseConnection();
            //dd.OpenConnection(Database.txt);
            //for (int i = 0; i < liste.Count; i++)
            //{
            //    if (sqlde_kayitli_urunler[i])
            //    {

            //    }
            //    komut = new SqlCommand(string.Format(@"delete from RpSubelerinSatilanlari where urun_adi=@urun_adi and sube_id='" + sube_id + "'"), dd.cn);
            //    komut.Parameters.Add(@"urun_adi", liste[i]);
            //    komut.ExecuteNonQuery();
            //}

            //dd.CloseConnection();
        }

    }
}
