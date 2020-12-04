using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scadenziario.EntityDto
{
public    class VociDto
    {
        public int IdVoce { get; set; }
        public int IdGruppo { get; set; }
        public string Gruppo { get; set; }
        public string Descrizione { get; set; }
        public DateTime? Scadenza { get; set; }
        public string ScadenzaStringa { get; set; }
        public decimal Importo { get; set; }
        public string ImportoStringa { get; set; }
        public int Evaso { get; set; }
    }
}
