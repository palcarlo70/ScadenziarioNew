using System;
using System.Data;
using System.Data.Common;
using DBAccess;

namespace Scadenziario.Connection
{
    public class ScadenzeCon : DBWork
    {
        public ScadenzeCon(string provider, string connectionString) : base(provider, connectionString)
        {
        }

        public DataSet GetVociOrdine(string idOrdine, int? idVoce)
        {
            DbCommand cmd = CreateCommand("SP_GetVoci", true);

            base.SetParameter(cmd, "Id", DbType.String, ParameterDirection.Input, (object)idOrdine ?? DBNull.Value);
            base.SetParameter(cmd, "GiornoDa", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "MeseDa", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "AnnoDa", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "GiornoA", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "MeseA", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "AnnoA", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);
            base.SetParameter(cmd, "Gruppo", DbType.Int32, ParameterDirection.Input, (object)idVoce ?? DBNull.Value);


            cmd.CommandType = CommandType.StoredProcedure;

            var lst = base.GetDataSet(cmd);

            return lst;
        }
    }
}
