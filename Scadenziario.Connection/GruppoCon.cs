using DBAccess;
using System;
using System.Data;
using System.Data.Common;
using Scadenziario.EntityDto;
using System.Collections.Generic;
using System.Linq;

namespace Scadenziario.Connection
{
    public class GruppoCon : DBWork
    {
        public GruppoCon(string Provider, string ConnectionString) : base(Provider, ConnectionString)
        {
        }

        private DataSet GetGruppiDs()
        {
            DbCommand cmd = CreateCommand("sp_GetGruppi", true);            

            cmd.CommandType = CommandType.StoredProcedure;
            var lst = base.GetDataSet(cmd);
            return lst;
        }

        public List<GruppoDto> GetGruppi()
        {

            var anods = new List<GruppoDto>();

            try
            {
                //var inDb = new AcquistiDac("System.Data.SqlClient", conAVdb);

                DataSet ds = GetGruppiDs();

                anods = (from DataRow dr in ds.Tables[0].Rows
                         select new GruppoDto()
                         {
                             IdGruppo = Convert.ToInt32(dr["Id"].ToString()),
                             Nome = dr["Nome"].ToString()
                         }).ToList();


            }
            catch (Exception ex )
            {
                return null;
            }
            return anods;
        }


    }
}
