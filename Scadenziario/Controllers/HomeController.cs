using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
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
        
        public ContentResult GetVoci(int? idVoce, string gruppo, string Data, string descri, int? evaso, int? daEvadere)
        {
            /*
             * GetVoci(int? idVoce, string gruppo,
            DateTime? DataDa,
            DateTime? DataAa,
            string descri, int? evaso, int? daEvadere)
             */

            DateTime? datDa = (DateTime?)null; if (!string.IsNullOrEmpty(Data)) datDa=Convert.ToDateTime("1/" + Data);
            var ultimo=System.DateTime.DaysInMonth((int)(datDa?.Year), (int)(datDa?.Month)).ToString();

            DateTime? datAa = (DateTime?)null; datAa = Convert.ToDateTime(ultimo+"/"+ Data);

            ClassiComuni clCom = new ClassiComuni();
            Connection.ScadenzeCon conn = new Scadenziario.Connection.ScadenzeCon("System.Data.SqlClient", clCom.ConnectDbpUniversal);


            EntityDto.VociGrigliaDto lst = conn.GetVoci(idVoce,gruppo,datDa,datAa,descri,evaso,daEvadere);

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
    }
}