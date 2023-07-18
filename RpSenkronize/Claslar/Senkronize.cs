using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RpSenkronize.Claslar
{
    public class Senkronize
    {
        Database db = new Database();
        Claslar.Subeler subeler = new Subeler();

        SqlCommand komut = new SqlCommand();
        SqlDataAdapter da = new SqlDataAdapter();
        SqlKayit kayit = new SqlKayit();
        Claslar.Loglar loglar = new Claslar.Loglar();

        public void genel_toplam_bilgileri(string baslangic, string bitis)
        {
            try
            {
                string gecici_urun_adi = "";
                List<string> satilan_urun_adlari = new List<string>();
                List<string> sqlde_kayitli_satilan_urunler = new List<string>();
                DateTime dateTime = DateTime.Now;
                DateTime dateTime2 = DateTime.Now;
                try
                {
                    dateTime = DateTime.ParseExact(baslangic, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    dateTime2 = DateTime.ParseExact(bitis, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    if (baslangic==null && bitis == null)
                    {
                  
                        string v1 = DateTime.Now.ToString("dd.MM.yyyy 00:00:00", System.Globalization.CultureInfo.InvariantCulture);
                        string v2 = DateTime.Now.ToString("dd.MM.yyyy 23:59:59", System.Globalization.CultureInfo.InvariantCulture);
                     


                        dateTime = Convert.ToDateTime(v1);
                        dateTime2 = Convert.ToDateTime(v2);
                    }
                    else
                    {
                        dateTime = Convert.ToDateTime(baslangic);
                        dateTime2 = Convert.ToDateTime(bitis);
                    }
                 

                }

                string gelen_baslangic = dateTime.ToString("yyyy-MM-dd HH:mm:ss");




                string gelen_bitis = dateTime2.ToString("yyyy-MM-dd HH:mm:ss");



                kayit.subelerin_cok_satilan_sil();
                kayit.satilan_urun_detaylarini_sil();
                kayit.genel_toplamlari_sifirla();
                kayit.subelerde_onceden_kalan_satilmayan_stoklari_sil();
                for (int i = 0; i < Degiskenler.connectionlar.Count; i++)
                {
                    satilan_urun_adlari.Clear();
                    sqlde_kayitli_satilan_urunler.Clear();
                    if (Degiskenler.connectionlar[i] == "Yok")
                    {
                        kayit.genel_toplam_kayit(Degiskenler.subeler[i], 0, 0, 0, 0, false);
                        kayit.subelerin_cok_satilan(Degiskenler.subeler[i], "", 0, false);
                        kayit.acik_masa_tutari(Degiskenler.subeler[i], 0, 0, false);
                        // kayit.subelerin_satilanlari(Degiskenler.subeler[i], "", 0, 0, false);
                    }
                    else
                    {
                        var genel_toplamlar = GenelToplamlar(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (genel_toplamlar.Read())
                        {
                            loglar.log_olustur("GenelToplamlar sorgusu çalıştı");
                            kayit.genel_toplam_kayit(Degiskenler.subeler[i], Convert.ToDecimal(genel_toplamlar["ToplamTutar"]), Convert.ToDecimal(genel_toplamlar["Toplamİade"]), Convert.ToDecimal(genel_toplamlar["Iskonto"]), Convert.ToDecimal(genel_toplamlar["İptal"]), true);
                            loglar.log_olustur("GenelToplamlar kayıt çalıştı");

                        }
                        var sube_en_cok_satilan = SubelerinCokSatilan(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (sube_en_cok_satilan.Read())
                        {
                            loglar.log_olustur("sube_en_cok_satilan sorgusu çalıştı");
                            kayit.subelerin_cok_satilan(Degiskenler.subeler[i], sube_en_cok_satilan["urunadi"].ToString(), int.Parse(sube_en_cok_satilan["sayi"].ToString()), true);
                            loglar.log_olustur("sube_en_cok_satilan kayıt çalıştı");
                        }
                        var acik_masa_tutarlari = AcikMasaTutarlari(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (acik_masa_tutarlari.Read())
                        {
                               loglar.log_olustur("acik_masa_tutarlari sorgusu çalıştı");
                            var deneme = acik_masa_tutarlari["sayi"].ToString();
                            var deneme2 = acik_masa_tutarlari["acik_masa_tutari"].ToString();
                            kayit.acik_masa_tutari(Degiskenler.subeler[i], Convert.ToDecimal(acik_masa_tutarlari["acik_masa_tutari"].ToString()), int.Parse(acik_masa_tutarlari["sayi"].ToString()), true);
                            loglar.log_olustur("acik_masa_tutarlari kayıt çalıştı");
                        }

                        var subede_satilanlar = SubelerinSatilanlari(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (subede_satilanlar.Read())
                        {

                            //gecici_urun_adi = subede_satilanlar["Level3_ad"].ToString();
                            //if (gecici_urun_adi != "")
                            //{

                            //}
                            if (int.Parse(subede_satilanlar["urunid"].ToString()) == 7)
                            {

                            }
                            satilan_urun_adlari.Add(subede_satilanlar["urunadi"].ToString());
                            kayit.subelerin_satilanlari(Degiskenler.subeler[i], subede_satilanlar["urunadi"].ToString(), int.Parse(subede_satilanlar["urunid"].ToString()), Convert.ToDecimal(subede_satilanlar["toplam_tutar"]), Convert.ToDecimal(subede_satilanlar["toplam_sayi"].ToString()), true);

                        }



                        var satilan_urun_detaylari = SatilanUrunDetaylari(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (satilan_urun_detaylari.Read())
                        {


                            gecici_urun_adi = satilan_urun_detaylari["Level3_ad"].ToString();
                            if (gecici_urun_adi == "")
                            {
                                gecici_urun_adi = "NORMAL";
                            }
                            kayit.satilan_urun_detaylari(Degiskenler.subeler[i], int.Parse(satilan_urun_detaylari["id"].ToString()), gecici_urun_adi, Convert.ToDecimal(satilan_urun_detaylari["toplam_sayi"]), Convert.ToDecimal(satilan_urun_detaylari["toplam_tutar"].ToString()), true);





                        }
                        var satis_kasa_hareketleri = SatisKasaHareketleri(Degiskenler.connectionlar[i], gelen_baslangic, gelen_bitis);
                        while (satis_kasa_hareketleri.Read())
                        {
                            kayit.satilan_kasa_hareketleri(Degiskenler.subeler[i], satis_kasa_hareketleri["kasa_adi"].ToString(), Convert.ToDecimal(satis_kasa_hareketleri["ToplamTutar"]));
                        }



                        // sube satırları silme
                    }

                }


            }
            catch (Exception ex)
            {

                loglar.log_olustur("genel_toplam_bilgileri clasında hata" + ex);
            }
           



        }
        public SqlDataReader GenelToplamlar(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(string.Format(@" select 
                                  t.ToplamTutar, 
                                  t.Toplamİade, 
                                  t.Iskonto, 
                                  t.İptal, 
                                  (t.ToplamTutar - t.Toplamİade) as NetTutar 
                                from 
                                  (
                                    select 
                                      (
                                        SELECT 
                                          ISNULL(
                                            SUM(satis.Gelir), 
                                            0
                                          ) AS ToplamTutar 
                                        FROM 
                                          BillSafe AS satis 
                                          LEFT OUTER JOIN SafeRegister AS kasa ON kasa.id = satis.Kasaid 
                                        WHERE 
                                          (
                                            satis.İzahat = 1 
                                            OR satis.İzahat = '12'
                                          ) 
                                        
                                          AND (satis.Kasaid > 0) 
                                          AND (kasa.kasaadi IS NOT NULL) 
                                          AND (
                                            satis.Tarih BETWEEN '{0}' 
                                            AND '{1}'
                                          )
                                         
                                      ) as ToplamTutar, 
                                      (
                                        SELECT 
                                          ISNULL(
                                            SUM(ToplamTutar), 
                                            0
                                          ) AS Toplamİade 
                                        FROM 
                                          Billinfo 
                                        WHERE 
                                          (BelgeTuru = 8) 
                                         
                                          AND (Kasaid > 0) 
                                          AND (
                                            BelgeTarihi BETWEEN '{0}'
                                            AND '{1}'
                                          )
                                         
                                      ) as Toplamİade, 
                                      (
                                        select 
                                          ISNULL(
                                            sum(İskonto), 
                                            0
                                          ) as İskonto 
                                        from 
                                          Bill as satis 
                                          inner join Billinfo as belge on satis.FisNo = belge.FisNo 
                                        where 
                                          (
                                            BelgeTuru = 1 
                                            or BelgeTuru = 12
                                          ) 
                                          and Kasaid > 0 
                                          and belge.BelgeTarihi BETWEEN '{0}' 
                                          AND '{1}'
                                         
                                      
                                      ) as Iskonto, 
                                      (
                                        select 
                                          ISNULL(
                                            sum(ToplamTutar), 
                                            0
                                          ) as İptal 
                                        from 
                                          Bill 
                                        where 
                                          İptal = 1 
                                          and BelgeTarihi BETWEEN '{0}' 
                                          AND '{1}'
                                         

                                          
                                        
                                      ) as İptal
                                  ) as t", baslangic, bitis), db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }
        public SqlDataReader SubelerinCokSatilan(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(@"select top 1 urun.urunadi,count(*) as sayi from PRODUCTS as urun inner join Bill as satis on satis.Urunid = urun.id where  barkod NOT IN ('NOT','ARA_ODEME','GETIR-ISKONTO') and BelgeTarihi between '" + baslangic + "' and '" + bitis + "' group by urun.urunadi order by sayi desc", db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }
        public SqlDataReader SubelerinSatilanlari(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(string.Format(@"SELECT 
                  urunadi,
                urunid,
                  
                  birim,
                   sum(Miktar) AS toplam_sayi,
                  SUM(Toplam) AS toplam_tutar
                FROM (
                  SELECT 
                    satis.miktar,
                   urun.id as urunid,
                    CASE WHEN satis.Level1_id > 0 THEN urun.urunadi + ' ' + bir.level1_ad ELSE urun.urunadi END AS urunadi, 
                    CASE WHEN urun.birim = 'ADET' THEN CAST(
                      CAST(
                        ISNULL(satis.Miktar, 0) - ISNULL(Returned.sayi, 0) AS DECIMAL(18, 2)
                      ) AS VARCHAR(20)
                    ) WHEN urun.birim = 'LT' THEN CAST(
                      CAST(
                        ISNULL(satis.Miktar, 0) - ISNULL(Returned.sayi, 0) AS DECIMAL(18, 2)
                      ) AS VARCHAR(20)
                    ) WHEN urun.birim = 'KG' THEN CAST(
                      CAST(
                        ISNULL(satis.Miktar, 0) - ISNULL(Returned.sayi, 0) AS DECIMAL(18, 3)
                      ) AS VARCHAR(20)
                    ) END AS sayi, 
                    urun.birim, 
                    urun.barkod, 
                    ISNULL(Returned.sayi, 0) AS iade_miktari, 
                    satis.ToplamTutar AS Toplam 
                  FROM 
                    PRODUCTS AS urun 
                    INNER JOIN Bill AS satis ON urun.id = satis.Urunid 
                    LEFT JOIN Returned ON satis.Urunid = Returned.id and Returned.BelgeTarihi BETWEEN '{0}' AND '{1}' 
                    LEFT JOIN level1 AS bir ON satis.Level1_id = bir.id 
                  WHERE satis.BelgeTarihi BETWEEN '{0}' AND '{1}'   AND 
                    satis.İptal = '0' 
                    AND satis.İade = '0' 
                and barkod NOT IN ('NOT','ARA_ODEME','GETIR-ISKONTO')

                ) AS subquery
                GROUP BY 
                  urunadi,
                urunid,
                  birim
                HAVING 
                  COUNT(*) > 1
                ORDER BY 
                  toplam_sayi DESC,
                  urunadi ASC
                ", baslangic, bitis), db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }
        public SqlDataReader AcikMasaTutarlari(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(@"select  ISNULL(sum(masa_kasa_tutari),0) as acik_masa_tutari, count(*) as sayi from  LastTableİnfo as info ", db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }
        public SqlDataReader SatilanUrunDetaylari(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(string.Format(@"	SELECT
                        p.urunadi,	p.id,
                        CASE
                            WHEN CHARINDEX(',', b.Level3_ad) > 0 THEN SUBSTRING(b.Level3_ad, 1, CHARINDEX(',', b.Level3_ad) - 1)
                            WHEN CHARINDEX('-', b.Level3_ad) > 0 THEN SUBSTRING(b.Level3_ad, 1, CHARINDEX('-', b.Level3_ad) - 1)
                            ELSE b.Level3_ad
                        END AS Level3_ad,
                         sum(Miktar) AS toplam_sayi,
                        SUM(b.ToplamTutar) AS toplam_tutar
                    FROM
                        Bill b
                        INNER JOIN PRODUCTS p ON p.id = b.Urunid
		                     where BelgeTarihi BETWEEN '{0}'    AND '{1}' and  İptal='0' and İade='0' and barkod NOT IN ('NOT','ARA_ODEME','GETIR-ISKONTO') 
                    GROUP BY
                        p.urunadi,	p.id,
                        CASE
                            WHEN CHARINDEX(',', b.Level3_ad) > 0 THEN SUBSTRING(b.Level3_ad, 1, CHARINDEX(',', b.Level3_ad) - 1)
                            WHEN CHARINDEX('-', b.Level3_ad) > 0 THEN SUBSTRING(b.Level3_ad, 1, CHARINDEX('-', b.Level3_ad) - 1)
                            ELSE b.Level3_ad
                        END
                    ORDER BY
                            toplam_sayi desc,
                        Level3_ad;", baslangic, bitis), db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }
        public SqlDataReader SatisKasaHareketleri(string connection, string baslangic, string bitis)
        {

            SqlDataReader dr;
            db.OpenConnection(connection);
            komut = new SqlCommand(string.Format(@"select 
                          kasa.kasaadi as kasa_adi, 
                          ISNULL(
                            sum(Gelir), 
                            0
                          ) as ToplamTutar 
                        from 
                          BillSafe as satis 
                          left join SafeRegister as kasa on kasa.id = satis.Kasaid 
                        where 
                          (
                            İzahat = 1 
                            or İzahat = '12'
                          ) 
                          and satis.Kasaid > 0 
                          and kasaadi IS NOT NULL 
                          and Tarih between '{0}' 
                          and '{1}' 
                        group by 
                          Kasaid, 
                          kasa.kasaadi
                          ", baslangic, bitis), db.cn);
            // komut.ExecuteNonQuery();
            dr = komut.ExecuteReader();
            return dr;

        }

    }
}
