using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace Scadenziario.Models
{
    public class ClassiComuni
    {
        private string _connectStringUniversal;
        public string ConnectDbpUniversal
        {
            get
            {
                ConnectionStringSettings mySetting = ConfigurationManager.ConnectionStrings["CarConnection"];
                if (string.IsNullOrEmpty(mySetting?.ConnectionString))
                {
                    _connectStringUniversal = null;
                }

                if (mySetting != null) _connectStringUniversal = mySetting.ConnectionString;
                else //creo erroe
                    throw new System.ArgumentException("Stringa di connessione non trovata DB DBP", ConnectDbpUniversal);

                return _connectStringUniversal;
            }
            set
            {
                _connectStringUniversal = value;
            }

        }
    }
}