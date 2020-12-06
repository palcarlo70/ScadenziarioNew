using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Scadenziario.EntityDto;
using Scadenziario.Models;

namespace Scadenziario.Controllers
{
    public class HomeController : Controller
    {
        readonly JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ContentResult GetGruppi()
        {
            ClassiComuni clCom = new ClassiComuni();
            Connection.GruppoCon conn = new Scadenziario.Connection.GruppoCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);

            List<EntityDto.GruppoDto> lst = conn.GetGruppi();

            return Content(JsonConvert.SerializeObject(lst, _jsonSetting), "application/json");
        }

        public ContentResult GetVoci(int? idVoce, string gruppo, string dataGiorno, string descri, int? evaso, int? daEvadere)
        {
            /*
             * GetVoci(int? idVoce, string gruppo,
            DateTime? DataDa,
            DateTime? DataAa,
            string descri, int? evaso, int? daEvadere)
             */
            if (string.IsNullOrEmpty(dataGiorno))
            {
                return Content(JsonConvert.SerializeObject(null, _jsonSetting), "application/json");
            }
            DateTime? datDa = (DateTime?)null; if (!string.IsNullOrEmpty(dataGiorno)) datDa = Convert.ToDateTime("1/" + dataGiorno);
            var ultimo = System.DateTime.DaysInMonth((int)(datDa?.Year), (int)(datDa?.Month)).ToString();

            DateTime? datAa = (DateTime?)null; datAa = Convert.ToDateTime(ultimo + "/" + dataGiorno);

            ClassiComuni clCom = new ClassiComuni();
            Connection.ScadenzeCon conn = new Scadenziario.Connection.ScadenzeCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);


            EntityDto.VociGrigliaDto lst = conn.GetVoci(idVoce, gruppo, datDa, datAa, descri, evaso, daEvadere);

            return Content(JsonConvert.SerializeObject(lst, _jsonSetting), "application/json");
        }



        public ContentResult GetRiepilogoAnnuale(string giorno)
        {
            ClassiComuni clCom = new ClassiComuni();
            Connection.ScadenzeCon conn = new Scadenziario.Connection.ScadenzeCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);

            var data = Convert.ToDateTime(giorno);

            var lst = conn.GetRiepilogoAnnuale(data);

            return Content(JsonConvert.SerializeObject(lst, _jsonSetting), "application/json");
        }

        public ContentResult InsUpDelRichieste(string dataRata, string descrizione, string importo, int idGruppo, int numRate, int cadenza)
        {
            var clCom = new ClassiComuni();
            var conn = new Scadenziario.Connection.ScadenzeCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);

            string[] lst = new string[2];

            decimal pre = 0;
            if (!string.IsNullOrEmpty(importo)) pre = Convert.ToDecimal(importo.Replace(".", ","));


            DateTime? dataOrd = string.IsNullOrEmpty(dataRata) ? (DateTime?)null : Convert.ToDateTime(dataRata);
            try
            {
                // popolo loggetto 
                var a = new VociDto
                {

                    IdGruppo = idGruppo,
                    Scadenza = dataOrd,
                    //ScadenzaStringa= dataRata,
                    Descrizione = descrizione,
                    Importo = pre,
                    NumRate = numRate,
                    Cadenza = cadenza
                };

                lst = conn.InsUpDelRichieste(a);

            }
            catch (Exception ex)
            {
                lst[1] = ex.Message;
            }
            return Content(JsonConvert.SerializeObject(lst, _jsonSetting), "application/json");
        }
        public ContentResult InsUpVoce(string dataRata, string descrizione, string importo, int idGruppo, int numRate, int cadenza, int elimina, int applicaATutti, int evaso)
        {
            var clCom = new ClassiComuni();
            var conn = new Scadenziario.Connection.ScadenzeCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);

            string[] lst = new string[2];

            decimal pre = 0;
            if (!string.IsNullOrEmpty(importo)) pre = Convert.ToDecimal(importo.Replace(".", ","));


            //public string[] InsUpVoci(VociDto v, int elimina, int applicaATutti)

            DateTime? dataOrd = string.IsNullOrEmpty(dataRata) ? (DateTime?)null : Convert.ToDateTime(dataRata);
            try
            {
                // popolo loggetto 
                var a = new VociDto
                {

                    IdGruppo = idGruppo,
                    Scadenza = dataOrd,                    
                    Descrizione = descrizione,
                    Importo = pre,
                    NumRate = numRate,
                    Cadenza = cadenza,
                    Evaso= evaso
                };

                lst = conn.InsUpVoci(a,elimina,applicaATutti);

            }
            catch (Exception ex)
            {
                lst[1] = ex.Message;
            }
            return Content(JsonConvert.SerializeObject(lst, _jsonSetting), "application/json");
        }
    }
}