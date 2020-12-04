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
                    vg.Giornata = DataDa?.AddDays(i).ToString("dd-MM");
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
                    vg.TolTipString = totl;
                    vg.Totale = totGiorno > 0 ? totGiorno.ToString("C") : string.Empty;

                    vociGiorni.Add(vg);
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
            vgTt.ImportoMedioGiorno = "0";  if ((Convert.ToDecimal((DataAa - DataDa)?.TotalDays) > 0)) vgTt.ImportoMedioGiorno =  (anods.Sum(c => c.Importo) / Convert.ToDecimal((DataAa - DataDa)?.TotalDays)).ToString("C");

            return vgTt;
        }

    }
}
