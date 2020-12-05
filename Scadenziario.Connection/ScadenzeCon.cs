using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using DBAccess;
using Scadenziario.EntityDto;
using System.Linq;

namespace Scadenziario.Connection
{
    public class ScadenzeCon : DBWork
    {
        public ScadenzeCon(string provider, string connectionString) : base(provider, connectionString)
        {
        }

        private DataSet GetVociDs(int? idVoce, string gruppo,
            int? giornoDa, int? meseDa, int? annoDa,
            int? giornoAa, int? meseAa, int? annoAa,
            string descri, int? evaso, int? daEvadere
            )
        {
            DbCommand cmd = CreateCommand("SP_GetVoci", true);

            base.SetParameter(cmd, "Id", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "GiornoDa", DbType.Int32, ParameterDirection.Input, (object)giornoDa ?? DBNull.Value);
            base.SetParameter(cmd, "MeseDa", DbType.Int32, ParameterDirection.Input, (object)meseDa ?? DBNull.Value);
            base.SetParameter(cmd, "AnnoDa", DbType.Int32, ParameterDirection.Input, (object)annoDa ?? DBNull.Value);
            base.SetParameter(cmd, "GiornoA", DbType.Int32, ParameterDirection.Input, (object)giornoAa ?? DBNull.Value);
            base.SetParameter(cmd, "MeseA", DbType.Int32, ParameterDirection.Input, (object)meseAa ?? DBNull.Value);
            base.SetParameter(cmd, "AnnoA", DbType.Int32, ParameterDirection.Input, (object)annoAa ?? DBNull.Value);
            base.SetParameter(cmd, "Gruppo", DbType.String, ParameterDirection.Input, (object)gruppo ?? DBNull.Value);
            base.SetParameter(cmd, "Descri", DbType.String, ParameterDirection.Input, (object)descri ?? DBNull.Value);
            base.SetParameter(cmd, "Evaso", DbType.Int32, ParameterDirection.Input, (object)evaso ?? DBNull.Value);
            base.SetParameter(cmd, "DaEvadere", DbType.Int32, ParameterDirection.Input, (object)daEvadere ?? DBNull.Value);


            cmd.CommandType = CommandType.StoredProcedure;

            var lst = base.GetDataSet(cmd);

            return lst;
        }

        public VociGrigliaDto GetVoci(int? idVoce, string gruppo,
            DateTime? DataDa,
            DateTime? DataAa,
            string descri, int? evaso, int? daEvadere)
        {

            var anods = new List<VociDto>();
            var vociGiorni = new List<VociCompresseMeseDto>();
            try
            {
                //var inDb = new AcquistiDac("System.Data.SqlClient", conAVdb);
                int? giornoDa = null;
                int? meseDa = null;
                int? annoDa = null;
                int? giornoAa = null;
                int? meseAa = null;
                int? annoAa = null;

                if (DataDa != null)
                {
                    giornoDa = DataDa.Value.Day;
                    meseDa = DataDa.Value.Month;
                    annoDa = DataDa.Value.Year;
                }
                if (DataAa != null)
                {
                    giornoAa = DataAa.Value.Day;
                    meseAa = DataAa.Value.Month;
                    annoAa = DataAa.Value.Year;
                }
                if (descri.Trim() == "") descri = null;
                if (gruppo.Trim() == "") gruppo = null;

                DataSet ds = GetVociDs(idVoce, gruppo, giornoDa, meseDa, annoDa, giornoAa, meseAa, annoAa, descri, evaso, daEvadere);

                anods = (from DataRow dr in ds.Tables[0].Rows
                         select new VociDto()
                         {
                             IdVoce = Convert.ToInt32(dr["Id"].ToString()),
                             IdGruppo = Convert.ToInt32(dr["IdGruppo"].ToString()),
                             Descrizione = dr["Descrizione"].ToString(),
                             Gruppo = dr["Nome"].ToString(),
                             Importo = Convert.ToDecimal(dr["Importo"].ToString()),
                             ImportoStringa = !dr.IsNull("Importo") ? Convert.ToDecimal(dr["Importo"].ToString()).ToString("C") : "0 €",
                             Scadenza = !dr.IsNull("Scadenza") ? DateTime.Parse(dr["Scadenza"].ToString()) : (DateTime?)null,
                             ScadenzaStringa = !dr.IsNull("Scadenza") ? DateTime.Parse(dr["Scadenza"].ToString()).ToString("dd/MM/yy") : string.Empty,
                             Evaso = Convert.ToInt32(dr["Saldato"].ToString())
                         }).ToList();



                //var oo = (DataAa - DataDa)?.TotalDays;
                for (int i = 0; i <= (DataAa - DataDa)?.TotalDays; i++)
                {
                    var vg = new VociCompresseMeseDto();

                    var lstVoci = anods.Where(c => c.Scadenza == DataDa?.AddDays(i)).ToList();
                    string totl = "";
                    decimal totGiorno = 0;
                    foreach (VociDto v in lstVoci)
                    {
                        if (totl != "") totl += Environment.NewLine;
                        var tit = v.Descrizione.Length <= 10 ? v.Descrizione : v.Descrizione.Substring(0, 10) + "...";
                        totl += $"{v.ScadenzaStringa} - {tit} - {v.ImportoStringa}";
                        totGiorno += v.Importo;
                    }

                    if (totGiorno != 0)
                    {
                        vg.Giornata = DataDa?.AddDays(i).ToString("dd-MM");
                        vg.TolTipString = totl;
                        vg.Totale = totGiorno > 0 ? totGiorno.ToString("C") : string.Empty;
                        vociGiorni.Add(vg);
                    }
                    
                }

                //Environment.NewLine

            }
            catch (Exception ex)
            {
                return null;
            }

            VociGrigliaDto vgTt = new VociGrigliaDto();
            vgTt.Voci = new List<VociDto>();
            vgTt.VociCompresse = new List<VociCompresseMeseDto>();
            vgTt.Voci = anods;
            vgTt.VociCompresse = vociGiorni;
            vgTt.ImportoTot = anods.Sum(c => c.Importo).ToString("C");
            vgTt.ImportoMedioGiorno = "0"; if ((Convert.ToDecimal((DataAa - DataDa)?.TotalDays) > 0)) vgTt.ImportoMedioGiorno = (anods.Sum(c => c.Importo) / Convert.ToDecimal((DataAa - DataDa)?.TotalDays)).ToString("C");

            return vgTt;
        }


        private DataSet GetRiepilogoAnnualeDs(DateTime giorno)
        {
            DbCommand cmd = CreateCommand("sp_GetRiepilogoAnnuale", true);

            base.SetParameter(cmd, "giorno", DbType.DateTime, ParameterDirection.Input, giorno);

            cmd.CommandType = CommandType.StoredProcedure;

            var lst = base.GetDataSet(cmd);

            return lst;
        }


        public AnnualeTot GetRiepilogoAnnuale(DateTime giorno)
        {

            string[] mesi = new string[13];
            mesi[1] = "GEN";
            mesi[2] = "FEB";
            mesi[3] = "MAR";
            mesi[4] = "APR";
            mesi[5] = "MAG";
            mesi[6] = "GIU";
            mesi[7] = "LUG";
            mesi[8] = "AGO";
            mesi[9] = "SET";
            mesi[10] = "OTT";
            mesi[11] = "NOV";
            mesi[12] = "DIC";


            var anno = new AnnualeTot();
            List<TitoliRiepiogoAnnoDto> tit = new List<TitoliRiepiogoAnnoDto>();
            var anods = new List<RiepiogoAnnoDto>();
            try
            {
                for (int i = 0; i < 12; i++)
                {
                    //var t = ('0' + giorno.AddMonths(i).Month.ToString());

                    tit.Add(new TitoliRiepiogoAnnoDto
                    {
                        Giorno = giorno.AddMonths(i).Month.ToString() + "/" + giorno.AddMonths(i).Year.ToString(),
                        Titolo = mesi[giorno.AddMonths(i).Month] + "-" + giorno.AddMonths(i).Year.ToString().Substring(2, 2)
                    });
                }


                DataSet ds = GetRiepilogoAnnualeDs(giorno);

                anods = (from DataRow dr in ds.Tables[0].Rows
                         select new RiepiogoAnnoDto()
                         {

                             Gruppo = dr["Gruppo"].ToString(),
                             Tipo = Convert.ToInt32(dr["Tipo"].ToString()),
                             Mese1Int = !dr.IsNull(2) ? Convert.ToDecimal(dr[2].ToString()) : 0,
                             Mese2Int = !dr.IsNull(3) ? Convert.ToDecimal(dr[3].ToString()) : 0,
                             Mese3Int = !dr.IsNull(4) ? Convert.ToDecimal(dr[4].ToString()) : 0,
                             Mese4Int = !dr.IsNull(5) ? Convert.ToDecimal(dr[5].ToString()) : 0,
                             Mese5Int = !dr.IsNull(6) ? Convert.ToDecimal(dr[6].ToString()) : 0,
                             Mese6Int = !dr.IsNull(7) ? Convert.ToDecimal(dr[7].ToString()) : 0,
                             Mese7Int = !dr.IsNull(8) ? Convert.ToDecimal(dr[8].ToString()) : 0,
                             Mese8Int = !dr.IsNull(9) ? Convert.ToDecimal(dr[9].ToString()) : 0,
                             Mese9Int = !dr.IsNull(10) ? Convert.ToDecimal(dr[10].ToString()) : 0,
                             Mese10Int = !dr.IsNull(11) ? Convert.ToDecimal(dr[11].ToString()) : 0,
                             Mese11Int = !dr.IsNull(12) ? Convert.ToDecimal(dr[12].ToString()) : 0,
                             Mese12Int = !dr.IsNull(13) ? Convert.ToDecimal(dr[13].ToString()) : 0
                         }).ToList();


                foreach (var t in anods)
                {
                    if (t.Mese1Int == 0) t.Mese1 = ""; else t.Mese1 = t.Mese1Int.ToString("C");
                    if (t.Mese2Int == 0) t.Mese2 = ""; else t.Mese2 = t.Mese2Int.ToString("C");
                    if (t.Mese3Int == 0) t.Mese3 = ""; else t.Mese3 = t.Mese3Int.ToString("C");
                    if (t.Mese4Int == 0) t.Mese4 = ""; else t.Mese4 = t.Mese4Int.ToString("C");
                    if (t.Mese5Int == 0) t.Mese5 = ""; else t.Mese5 = t.Mese5Int.ToString("C");
                    if (t.Mese6Int == 0) t.Mese6 = ""; else t.Mese6 = t.Mese6Int.ToString("C");
                    if (t.Mese7Int == 0) t.Mese7 = ""; else t.Mese7 = t.Mese7Int.ToString("C");
                    if (t.Mese8Int == 0) t.Mese8 = ""; else t.Mese8 = t.Mese8Int.ToString("C");
                    if (t.Mese9Int == 0) t.Mese9 = ""; else t.Mese9 = t.Mese9Int.ToString("C");
                    if (t.Mese10Int == 0) t.Mese10 = ""; else t.Mese10 = t.Mese10Int.ToString("C");
                    if (t.Mese11Int == 0) t.Mese11 = ""; else t.Mese11 = t.Mese11Int.ToString("C");
                    if (t.Mese12Int == 0) t.Mese12 = ""; else t.Mese12 = t.Mese12Int.ToString("C");
                }

                anno.Riepilogo = anods;
                anno.Titoli = tit;
            }
            catch (Exception ex)
            {
                return null;
            }

            return anno;
        }

    }
}
