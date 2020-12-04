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

    }
}