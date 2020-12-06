using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scadenziario.EntityDto
{
    public class VociDto
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
        public int NumRate { get; set; }
        public int Cadenza { get; set; }
    }

    public class VociCompresseMeseDto
    {
        
        public string Giornata { get; set; }
        public string Totale { get; set; }        
        public string TolTipString { get; set; }
        
    }

    public class VociGrigliaDto
    {
        public List<VociDto> Voci { get; set; }
        public List<VociCompresseMeseDto> VociCompresse { get; set; }        
        public string ImportoTot { get; set; }
        public string ImportoMedioGiorno { get; set; }
    }

    public class RiepiogoAnnoDto
    {
        public string Gruppo { get; set; }
        public int Tipo { get; set; }
        public string Mese1 { get; set; }
        public string Mese2 { get; set; }
        public string Mese3 { get; set; }
        public string Mese4 { get; set; }
        public string Mese5 { get; set; }
        public string Mese6 { get; set; }
        public string Mese7 { get; set; }
        public string Mese8 { get; set; }
        public string Mese9 { get; set; }
        public string Mese10 { get; set; }
        public string Mese11 { get; set; }
        public string Mese12 { get; set; }

        public decimal Mese1Int { get; set; }
        public decimal Mese2Int { get; set; }
        public decimal Mese3Int { get; set; }
        public decimal Mese4Int { get; set; }
        public decimal Mese5Int { get; set; }
        public decimal Mese6Int { get; set; }
        public decimal Mese7Int { get; set; }
        public decimal Mese8Int { get; set; }
        public decimal Mese9Int { get; set; }
        public decimal Mese10Int { get; set; }
        public decimal Mese11Int { get; set; }
        public decimal Mese12Int { get; set; }

    }

    public class TitoliRiepiogoAnnoDto
    {
        public string Giorno { get; set; }
        public string Titolo { get; set; }
    }

    public class AnnualeTot
    {
        public List<RiepiogoAnnoDto> Riepilogo { get; set; }
        public List<TitoliRiepiogoAnnoDto> Titoli { get; set; }
    }
}
