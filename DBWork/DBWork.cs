using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace DBAccess
{
    /// <summary>
    /// classe la quale fornisce tutto ciò
    /// che riguarda i contenitori dati
    /// e il collegamento con il DB
    /// </summary>
    public class DBWork
    {
        private DbProviderFactory _Factory;
        private DbDataReader _Reader = null;
        public DbConnection _Connection;
        public DbTransaction _Transaction;

        /// <summary>
        /// costrutture che carica 
        /// i paramentri direttamente dal fine .config
        /// </summary>
        public DBWork()
        {
            string Provider = ConfigurationManager.AppSettings["Provider"];
            this._Factory = DbProviderFactories.GetFactory(Provider);
            this._Connection = _Factory.CreateConnection();
            this._Connection.ConnectionString = ConfigurationManager.ConnectionStrings["DbConn"].ConnectionString;
            
        }

        /// <summary>
        /// costruttore che carica i paramenti 
        /// dal file .config
        /// </summary>
        /// <param name="Provider">nome del provider
        /// Es. del tag nel .config <add key="Provider" value="System.Data.SqlClient"/></param>
        /// <param name="ConnectionString">nome della stringa di connessione
        /// Es. del tag nel  <add name="DbConn" connectionString="" providerName="System.Data.SqlClient"/></param>
        public DBWork(string Provider, string ConnectionString)
        {
            this._Factory = DbProviderFactories.GetFactory(Provider);
            this._Connection = _Factory.CreateConnection();
            //this._Connection = DbProviderFactories.GetFactory(Provider).CreateConnection();
            this._Connection.ConnectionString = ConnectionString;

        }


        /// <summary>
        /// Inizia una nuova transazione nel DB.
        /// </summary>
        /// <seealso cref="CommitTransaction"/>
        /// <seealso cref="RollbackTransaction"/>
        /// <returns>Un object rappresentante una nuova transazione.</returns>
        public DbTransaction BeginTransaction()
        {
            this.CheckTransactionState(false);
            this._Transaction = this._Connection.BeginTransaction();
            return this._Transaction;
        }

        /// <summary>
        /// Inizia una nuova transazione nel DB specificando 
        /// il transaction isolation level.
        /// <seealso cref="CommitTransaction"/>
        /// <seealso cref="RollbackTransaction"/>
        /// </summary>
        /// <param name="isolationLevel">Il transaction isolation level.</param>
        /// <returns>Un  oggetto rappresentante una nuova transazione.</returns>
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            this.CheckTransactionState(false);
            this._Transaction = this._Connection.BeginTransaction(isolationLevel);
            return this._Transaction;
        }

        /// <summary>
        /// Commit della transazione corrente nel database.
        /// <seealso cref="BeginTransaction"/>
        /// <seealso cref="RollbackTransaction"/>
        /// </summary>
        public void CommitTransaction()
        {
            this.CheckTransactionState(true);
            this._Transaction.Commit();
            this._Transaction = null;
        }


        /// <summary>
        /// Rollback della transazione corrente nel database.
        /// <seealso cref="BeginTransaction"/>
        /// <seealso cref="CommitTransaction"/>
        /// </summary>
        public void RollbackTransaction()
        {
            this.CheckTransactionState(true);
            this._Transaction.Rollback();
            this._Transaction = null;
        }



        /// <summary>
        /// Verifica lo stato della transazione corrente
        /// </summary>
        /// <param name="mustBeOpen">variabile booleana
        /// 1- TRUE Transazione aperta
        /// 2 - FALSE Transazione chiusa</param>
        private void CheckTransactionState(bool mustBeOpen)
        {
            if (mustBeOpen)
            {
                if (null == _Transaction)
                    throw new InvalidOperationException("Transazione non aperta.");
            }
            else
            {
                if (null != _Transaction)
                    throw new InvalidOperationException("Transazione già aperta.");
            }
        }


        /// <summary>
        /// metodo generic usato per effettuare un ExecuteScalar
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd">tipo di dato del valore di output</param>
        /// <param name="WithTransaction">variabile boolena
        /// 1 - TRUE Il metodo ha bisogno di un oggetto <see cref="System.Data.Common.DbCommand"/></param>
        ///     con una connessione ed una transazione aperta 
        /// 2 - FALSE Il metodo ha bisogno solo di un oggetto <see cref="System.Data.Common.DbCommand"/>
        /// <returns>incrocio dei pali</returns>
        public virtual T CreateCommandScalar<T>(DbCommand cmd, bool WithTransaction)
        {
            T iNREC = default(T);

            if (!WithTransaction)
                this.Open();

            iNREC = (T)cmd.ExecuteScalar();

            if (!WithTransaction)
                this.Close();

            return iNREC;
        }

        /// <summary>
        /// metodo che esegue un ExecuteNonQuery
        /// </summary>
        /// <param name="cmd">tipo di dato del valore di output</param>
        /// <param name="WithTransaction">variabile boolena
        /// 1 - Il metodo ha bisogno di un oggetto <see cref="System.Data.Common.DbCommand"/></param>
        ///     con una connessione ed una transazione aperta
        /// 2 - FALSE Il metodo ha bisogno solo di un oggetto <see cref="System.Data.Common.DbCommand"/>
        /// <returns>numero dei record coinvolti</returns>
        public virtual int CreateCommandNonQuery(DbCommand cmd, bool WithTransaction)
        {
            int iNREC = 0;
            if (!WithTransaction)
                this.Open();

            iNREC = cmd.ExecuteNonQuery();
            if (!WithTransaction)
                this.Close();
            return iNREC;
        }


        /// <summary>
        /// metodo che crea un oggetto <see cref="System.Data.Common.DbCommand"/>
        /// </summary>
        /// <param name="sqlText">striga contenente l'SQL del Command</param>
        /// <returns><see cref="System.Data.Common.DbCommand"/></returns>
        public DbCommand CreateCommand(string sqlText)
        {
            return CreateCommand(sqlText, false, 0);
        }

        /// <summary>
        /// (overload) metodo che crea un oggetto <see cref="System.Data.Common.DbCommand"/>
        /// </summary>
        /// <param name="sqlText">striga contenente l'SQL del Command</param>
        /// <param name="TimeOut">valore del timeout della query</param>
        /// <returns><see cref="System.Data.Common.DbCommand"/></returns>
        public DbCommand CreateCommand(string sqlText, int TimeOut)
        {
            return CreateCommand(sqlText, false, TimeOut);
        }


        /// <summary>
        /// Crea e ritorna un nuovo <see cref="System.Data.Common.DbCommand"/>
        /// </summary>
        /// <param name="sqlText">Testo della query.</param>
        /// <param name="procedure">Specifica se il testo della query 
        /// è il nome di una stored procedure.</param>
        /// <param name="TimeOut">variabile int setta la proprietà CommandTimeout</param>
        /// <returns><see cref="System.Data.Common.DbCommand"/></returns>
        public DbCommand CreateCommand(string sqlText, bool procedure, int TimeOut)
        {
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlText;
            cmd.Transaction = _Transaction;
            cmd.CommandTimeout = TimeOut;
            if (procedure)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }


        /// <summary>
        /// Overload Crea e restituisce un nuovo <see cref="System.Data.Common.DbCommand"/>
        /// </summary>
        /// <param name="sqlText">Testo della query.</param>
        /// <param name="procedure">Specifica se il testo della query 
        /// è il nome di una stored procedure.</param>
        /// e gli passo null non setta la proprietà CommandTimeout</param>
        /// <returns><see cref="System.Data.Common.DbCommand"/></returns>
        public DbCommand CreateCommand(string sqlText, bool procedure)
        {
            DbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = sqlText;
            cmd.Transaction = _Transaction;
            if (procedure)
                cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }


        /// <summary>
        /// metodo che imposta gli oggetti <see cref="System.Data.Common.DbParameter"/>
        /// </summary>
        /// <typeparam name="T">Tipo di dato per il valore del paramentro (cast runtime)</typeparam>
        /// <param name="cmd"><see cref="System.Data.Common.DbCommand"/></param>
        /// <param name="paramName">Nome del parametro</param>
        /// <param name="dbType">enumeratore con il tipo di parametro <see cref="System.Data.DbType"/></param>
        /// <param name="ParDir">enumeratore con la direzione del parametro <see cref=" System.Data.ParameterDirection"/></param>
        /// <param name="value">parametro generico contenente il valore contenuto nel paramentro</param>
        /// <returns><see cref="System.Data.Common.DbParameter"/></returns>
        public virtual DbParameter SetParameter<T>(DbCommand cmd, string paramName, DbType dbType, ParameterDirection ParDir, T value)
        {
            DbParameter parameter = cmd.CreateParameter();
            parameter.DbType = dbType;
            parameter.ParameterName = paramName;
            parameter.Direction = ParDir;
            if (value == null)
                parameter.Value = DBNull.Value;
            else
                parameter.Value = value;
            cmd.Parameters.Add(parameter);
            return parameter;
        }



        /// <summary>
        /// metodo che restituisce un DataReader <see cref="System.Data.Common.DbDataReader"/>
        /// per chiudere il DataReader usare il metodo
        /// CloseDataReader
        /// </summary>
        /// <param name="cmd"><see cref="System.Data.Common.DbCommand"/></param>
        /// <returns><see cref="System.Data.Common.DbDataReader"/></returns>
        public virtual DbDataReader GetDataReader(DbCommand cmd)
        {
            this.Open();
            this._Reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return this._Reader;
        }

        /// <summary>
        /// metodo che restituisce un DataTable <see cref="System.Data.DataTable"/>
        /// </summary>
        /// <param name="cmd"><see cref="System.Data.Common.DbCommand"/></param>
        /// <returns><see cref="System.Data.DataTable"/></returns>
        public virtual DataTable ExcuteDataTable(DbCommand cmd)
        {
            int Errore = 0;
            string MessaggioErrore = "";

            SetParameter<int>(cmd, "Errore", DbType.Int32, ParameterDirection.Output, Errore);
            SetParameter<string>(cmd, "MessaggioErrore", DbType.String, ParameterDirection.Output, MessaggioErrore);

            DataTable dt = GetDataTable(cmd);

            Errore = Convert.ToInt32(cmd.Parameters["Errore"].Value);
            MessaggioErrore = cmd.Parameters["MessaggioErrore"].Value.ToString();

            if (Errore != 0)
            {
                throw (new Exception(Errore.ToString() + ": " + MessaggioErrore));
            }

            return dt;
        }

        /// <summary>
        /// metodo che restituisce un DataTable <see cref="System.Data.DataTable"/>
        /// </summary>
        /// <param name="cmd"><see cref="System.Data.Common.DbCommand"/></param>
        /// <returns><see cref="System.Data.DataTable"/></returns>
        public virtual DataTable GetDataTable(DbCommand cmd)
        {
            DataTable dt = new DataTable();
            using (_Connection)
            {
                using (DbDataAdapter oleDA = _Factory.CreateDataAdapter())
                {
                    //_Connection.Open();
                    oleDA.SelectCommand = cmd;
                    cmd.Connection = _Connection;
                    oleDA.Fill(dt);

                }
            }
            return dt;
        }

        /// <summary>
        /// Esegue un SqlCommand aggiungendo la gestione dell'errore restituito dalla stored procedure eseguita
        /// </summary>
        /// <param name="objCmd">SqlCommand da eseguire</param>
        /// <returns>DataSet con i dati risultato della stored procedure</returns>
        public virtual DataSet ExcuteSQLDataSet(DbCommand cmd)
        {
            int Errore = 0;
            string MessaggioErrore = "";

            SetParameter<int>(cmd, "Errore", DbType.Int32, ParameterDirection.Output, Errore);
            SetParameter<string>(cmd, "MessaggioErrore", DbType.String, ParameterDirection.Output, MessaggioErrore);

            DataSet ds = GetDataSet(cmd);

            Errore = Convert.ToInt32(cmd.Parameters["Errore"].Value);
            MessaggioErrore = cmd.Parameters["MessaggioErrore"].Value.ToString();

            if (Errore != 0)
            {
                throw (new Exception(Errore.ToString() + ": " + MessaggioErrore));
            }

            return ds;
        }

        /// <summary>
        /// metodo che ristituisce un DataSet <see cref="System.Data.DataSet"/>
        /// </summary>
        /// <param name="cmd"><see cref="System.Data.SqlClient.SqlCommand"/></param>
        /// <returns><see cref="System.Data.DataSet"/></returns>
        public virtual DataSet GetDataSet(DbCommand cmd)
        {
            DataSet ds = new DataSet();
            try
            {
                using (_Connection)
                {
                    using (DbDataAdapter oleDA = _Factory.CreateDataAdapter())
                    {
                        //_Connection.Open();
                        oleDA.SelectCommand = cmd;
                        cmd.Connection = _Connection;
                        oleDA.Fill(ds);
                    }
                }
            }
            catch (Exception)
            {

            }
            return ds;
        }

        /// <summary>
        /// Chiude un oggetto 
        /// <see cref="System.Data.Common.DbDataReader"/>
        /// </summary>
        public void CloseDataReader()
        {
            if (null != this._Reader && !this._Reader.IsClosed)
                this._Reader.Close();
        }

        /// <summary>
        /// Apre la connessione al DB connection.
        /// </summary>
        public void Open()
        {
            if (null != this._Connection && this._Connection.State == ConnectionState.Closed)
               this. _Connection.Open();
        }

        /// <summary>
        /// Chiude la connessione al DB connection.
        /// </summary>
        public void Close()
        {
            if (null != _Connection && this._Connection.State == ConnectionState.Open)
                this._Connection.Close();
        }

        /// <summary>
        /// Chiude la connessione al DB connection
        /// e ritorna indietro con le transazioni pendenti
        /// </summary>
        public void Dispose()
        {
            this.Close();
            if (null != this._Connection)
            {
                this._Connection.Dispose();
                GC.Collect();
            }
        }
    }
}	

